namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IMessageRepository MessageRepository { get; }
        IUserLikeRepository UserLikeRepository { get; }
        IUserRepository UserRepository { get; }
        Task<bool> CompleteAsync();
        bool HasChanges();
    }
}
