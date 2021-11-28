using DotWriterServer.Dto;

namespace DotWriterServer.Services
{
    public interface IDotWriterService
    {
        WebResponse<bool> Execute(string base64Image);
    }
}