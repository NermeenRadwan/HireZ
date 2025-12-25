using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using HireZ.Data;
using HireZ.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HireZ.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly ApplicationDbContext _db;
        public FeedbackService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<List<FeedbackDto>> GetFeedbackForResumeAsync(int resumeId)
        {
            // Load feedback entities for the resume
            var feedbacks = await _db.ResumeFeedbacks
                .Where(f => f.ResumeId == resumeId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync();

            return feedbacks.Select(MapToDto).ToList();
        }

        public async Task<FeedbackDto?> GetFeedbackByIdAsync(int feedbackId)
        {
            var f = await _db.ResumeFeedbacks.FindAsync(feedbackId);
            if (f == null) return null;
            return MapToDto(f);
        }

        /// <summary>
        /// Map ResumeFeedback entity to FeedbackDto using reflection to extract common textual fields.
        /// This keeps compatibility with varying model property names (FeedbackText, Content, Analysis, etc.).
        /// </summary>
        private FeedbackDto MapToDto(object feedbackEntity)
        {
            // Using reflection to extract common properties
            var type = feedbackEntity.GetType();
            int id = (int)type.GetProperty("Id")!.GetValue(feedbackEntity)!;
            int resumeId = (int)type.GetProperty("ResumeId")!.GetValue(feedbackEntity)!;
            DateTime createdAt = DateTime.MinValue;
            var createdProp = type.GetProperty("CreatedAt") ?? type.GetProperty("Created");
            if (createdProp != null)
            {
                var val = createdProp.GetValue(feedbackEntity);
                if (val is DateTime dt) createdAt = dt;
            }

            // try to find a textual property
            string? content = null;
            string? source = null;
            var candidateNames = new[] { "FeedbackText", "Content", "Analysis", "Text", "Body" };
            foreach (var name in candidateNames)
            {
                var prop = type.GetProperty(name);
                if (prop != null && prop.PropertyType == typeof(string))
                {
                    content = prop.GetValue(feedbackEntity) as string;
                    if (!string.IsNullOrWhiteSpace(content)) break;
                }
            }

            // source
            var sourceProp = type.GetProperty("Source") ?? type.GetProperty("Origin");
            if (sourceProp != null && sourceProp.PropertyType == typeof(string))
            {
                source = sourceProp.GetValue(feedbackEntity) as string;
            }

            return new FeedbackDto
            {
                Id = id,
                ResumeId = resumeId,
                CreatedAt = createdAt == DateTime.MinValue ? DateTime.UtcNow : createdAt,
                Content = content,
                Source = source
            };
        }
    }
}
