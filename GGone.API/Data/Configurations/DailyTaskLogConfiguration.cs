using GGone.API.Models.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace GGone.API.Data.Configurations
{
    public class DailyTaskLogConfiguration : IEntityTypeConfiguration<DailyTaskLog>
    {
        public void Configure(EntityTypeBuilder<DailyTaskLog> builder)
        {
            // Liste içeriğindeki değişiklikleri takip etmek için comparer tanımlıyoruz
            var comparer = new ValueComparer<List<int>>(
                (c1, c2) => c1.SequenceEqual(c2), // İki liste aynı mı?
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())), // Hash kodu üretimi
                c => c.ToList()); // Listenin kopyasını al

            builder.Property(e => e.CompletedTaskIds)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                    v => JsonSerializer.Deserialize<List<int>>(v, (JsonSerializerOptions)null) ?? new List<int>()
                )
                .Metadata.SetValueComparer(comparer); // Comparer'ı burada bağlıyoruz
        }
    }
}

