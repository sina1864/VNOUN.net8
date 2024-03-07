using AutoMapper;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson;
using Vnoun.Application.Requests.Auth;
using Vnoun.Application.Requests.Locations;
using Vnoun.Application.Requests.MetaRequests;
using Vnoun.Application.Requests.Notifications;
using Vnoun.Application.Requests.Post;
using Vnoun.Application.Responses.Auth;
using Vnoun.Application.Responses.Cards;
using Vnoun.Application.Responses.Category;
using Vnoun.Application.Responses.Event;
using Vnoun.Application.Responses.Locations;
using Vnoun.Application.Responses.MetaResponses;
using Vnoun.Application.Responses.Notifications;
using Vnoun.Application.Responses.Post;
using Vnoun.Application.Responses.Product;
using Vnoun.Application.Responses.Reviews;
using Vnoun.Application.Responses.User;
using Vnoun.Application.Responses.Wishlist;
using Vnoun.Core.Entities;
using Vnoun.Core.Entities.MetaEntities;
using Vnoun.Core.Repositories.Dtos.Requests.Notifications;
using Post = Vnoun.Core.Entities.Post;

namespace Vnoun.Application.Mappers;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<User, SignupRequestDto>().ReverseMap();

        CreateMap<IFormFile, List<CoverImage>>()
            .ConvertUsing((formFile, _, context) => GetCoverImagesFromFormFile(formFile, context));

        CreateMap<CoverImage, CoverImageCreateRequest>().ReverseMap();
        CreateMap<CoverImage, CoverImageCreateResponse>().ReverseMap();

        CreateMap<CoverImage, CoverImageResponse>()
            .ForMember(dest => dest._id, opt => opt.MapFrom(src => src.Id))
            .ReverseMap();

        CreateMap<Photo, PhotoResponseDto>()
            .ForMember(dest => dest._id, opt => opt.MapFrom(src => src.ID))
            .ReverseMap();

        CreateMap<PostImage, PhotoResponseDto>()
            .ForMember(dest => dest._id, opt => opt.MapFrom(src => src.Id))
            .ReverseMap();

        CreateMap<CoverImage, PhotoResponseDto>()
            .ForMember(dest => dest._id, opt => opt.MapFrom(src => src.Id))
            .ReverseMap();

        CreateMap<User, UserResponseDto>()
            .ForMember(dest => dest._id, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ID))
            .ForMember(des => des.Photo, opt => opt.MapFrom(ss => ss.Photo))
            .ForMember(des => des.Bag, opt => opt.MapFrom(ss => ss.Bag))
            .ForMember(des => des.Wishlist, opt => opt.MapFrom(ss => ss.Wishlist))
            .ReverseMap();

        CreateMap<Category, CategoryResponseDto>()
            .ForMember(dest => dest._id, opt => opt.MapFrom(src => src.ID))
            .ReverseMap();

        CreateMap<Product, ProductResponseDto>()
            .ForMember(dest => dest._id, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.BuyersId, opt => opt.MapFrom(src => src.Buyers))
            .ReverseMap();

        CreateMap<Event, EventResponseDto>()
            .ForMember(dest => dest._id, opt => opt.MapFrom(src => src.ID))
            .ReverseMap();

        CreateMap<Post, PostResponseDto>()
            .ForMember(dest => dest._id, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => src.Publisher))
            .ForMember(dest => dest.CoverImage, opt => opt.MapFrom(src => src.CoverImage))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.Images))
            .ReverseMap();

        CreateMap<PostCreateRequestDto, Post>()
            .ForMember(dest => dest.CoverImage, opt => opt.MapFrom(src => new List<CoverImage>()))
            .ForMember(dest => dest.Images, opt => opt.MapFrom(src => new List<PostImage>()))
            .ReverseMap();

        CreateMap<User, DetailedUserResponseDto>()
            .ForMember(dest => dest.Photos, opt => opt.MapFrom(src => src.Photo))
            .ForMember(dest => dest._id, opt => opt.MapFrom(src => src.ID.ToString()))
            .ReverseMap();

        CreateMap<PushNotificationRequestDto, PushNotificationServiceRequestDto>()
            .ReverseMap();

        CreateMap<Notification, NotificationResponse>()
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.UserId))
            .ReverseMap();

        CreateMap<Notification, NotificationResponseWithUser>()
            .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            .ReverseMap();

        CreateMap<User, NotificationUserResponseDto>()
            .ForMember(dest => dest._id, opt => opt.MapFrom(src => src.ID.ToString()))
            .ForMember(dest => dest.Photo, opt => opt.MapFrom(src => src.Photo))
            .ReverseMap();

        CreateMap<LocationInfoRequestDto, InformationLocation>()
            .ReverseMap();

        CreateMap<CreateLocationRequestDto, Location>()
            .ForMember(dest => dest.Information, opt => opt.MapFrom(src => src.Information))
            .ReverseMap();

        CreateMap<Location, LocationResponseDto>()
            .ForMember(dest => dest._id, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.Information, opt => opt.MapFrom(src => src.Information))
            .ReverseMap();

        CreateMap<InformationLocation, LocationInfoResponseDto>()
            .ReverseMap();

        CreateMap<Card, CardsResponseDto>()
            .ForMember(dest => dest._id, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
            .ReverseMap();

        CreateMap<Review, ReviewsResponseDto>()
            .ForMember(dest => dest._id, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ReverseMap();

        CreateMap<Wishlist, WishListResponseDto>()
            .ForMember(dest => dest._id, opt => opt.MapFrom(src => src.ID))
            .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
            .ReverseMap();
    }

    private static List<CoverImage> GetCoverImagesFromFormFile(IFormFile? formFile, ResolutionContext context)
    {
        if (formFile == null)
            return new List<CoverImage>();

        var smallImage = "small-" + formFile.Name;
        var mediumImage = "medium-" + formFile.Name;
        var largeImage = "large-" + formFile.Name;

        var coverImage = new CoverImage
        {
            Id = ObjectId.GenerateNewId().ToString(),
            SmallImage = smallImage,
            MediumImage = mediumImage,
            LargeImage = largeImage
        };

        return new List<CoverImage> { coverImage };
    }
}