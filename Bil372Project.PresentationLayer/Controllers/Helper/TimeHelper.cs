using System;

namespace Bil372Project.PresentationLayer.Controllers.Helper
{
    public static class TimeHelper
    {
        public static string GetRelativeTimeText(DateTime? updatedAt)
        {
            if (!updatedAt.HasValue)
                return "Henüz hiç güncellenmedi";

            var localUpdated = updatedAt.Value.ToLocalTime();
            var now = DateTime.Now;
            var diff = now - localUpdated;

            if (diff.TotalMinutes < 1)
                return "Az önce";

            if (diff.TotalHours < 1)
                return $"{(int)diff.TotalMinutes} dakika önce";

            if (diff.TotalDays < 1)
                return $"{(int)diff.TotalHours} saat önce";

            if (diff.TotalDays < 7)
                return $"{(int)diff.TotalDays} gün önce";

            return localUpdated.ToString("dd.MM.yyyy");
        }
    }
}