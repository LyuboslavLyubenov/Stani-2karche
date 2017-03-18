namespace Interfaces
{

    using DTOs;

    public interface ISelectPlayerTypeUIController<in T> : ISelectPlayerTypeUIController where T : CreatedGameInfo_DTO
    {
        void Initialize(T gameInfoDto);
    }

    public interface ISelectPlayerTypeUIController
    {
        void Initialize(object gameInfoDto);
    }

}