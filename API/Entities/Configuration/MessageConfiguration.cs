using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace API.Entities.Configuration
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.HasOne(x=>x.Sender).WithMany(x=>x.MessageSent).HasForeignKey(x=>x.SenderId).OnDelete(DeleteBehavior.ClientSetNull);
            builder.HasOne(x=>x.Reciever).WithMany(x=>x.MessageRecieved).HasForeignKey(x => x.RecieverId).OnDelete(DeleteBehavior.ClientSetNull);

        }
    }
}
