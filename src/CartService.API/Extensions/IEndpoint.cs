namespace CartService.API.Extensions;

public interface IEndpoint
{
    static abstract void MapEndpoints(IEndpointRouteBuilder app);
}
