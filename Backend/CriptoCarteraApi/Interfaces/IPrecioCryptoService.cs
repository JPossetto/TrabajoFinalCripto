namespace CriptoCarteraApi.Interfaces
{
    public interface IPrecioCryptoService
    {
        Task<decimal> ObtenerPrecio(string cryptoCode, string action);
    }
}
