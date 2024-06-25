using API.DTOS;
using API.Entities;
using AutoMapper;

namespace API.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<User, MemberDto>()
                //.ForMember(x=>x.DateOfBirth , c=>c.MapFrom(v=>v.DateOfBirth.Date))
                .ForMember(x=>x.Age , C=>C.MapFrom(v=>v.GetAge()))
                .ForMember(x=>x.PhotoUrl , c=>c.MapFrom(v=>v.Photos.FirstOrDefault(b=>b.IsMain).Url));

            CreateMap<Photo, PhotoDto>();
            CreateMap<UpdateUserDto, User>();
            CreateMap<registerDto, User>();
            CreateMap<User , LikeDto>()
                .ForMember(x=>x.PictureUrl , c=>c.MapFrom(v=>v.PhotoUrl))
                .ForMember(x=>x.Age , c=>c.MapFrom(v=>v.GetAge()));
            CreateMap<Message, MessageDto>()
               .ForMember(x => x.RecieverPhotoUrl, c => c.MapFrom(v => v.Reciever.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(x => x.SenderPhotoUrl, c => c.MapFrom(v => v.Sender.Photos.FirstOrDefault(x => x.IsMain).Url));
            CreateMap<MessageDto, Message>();
        }
    }
}
