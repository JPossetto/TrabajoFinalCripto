using CriptoCarteraApi.Interfaces;
using System.Text.Json;

namespace CriptoCarteraApi.Services
{
    public class PrecioCryptoService : IPrecioCryptoService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public PrecioCryptoService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<decimal> ObtenerPrecio(string cryptoCode, string action)
        {
            var codigoApi = PasarCodigoParaCriptoYa(cryptoCode);
            var client = _httpClientFactory.CreateClient();
            var url = $"https://criptoya.com/api/satoshitango/{codigoApi}/ars/1";

            var respuesta = await client.GetAsync(url);
            respuesta.EnsureSuccessStatusCode();

            var json = await respuesta.Content.ReadAsStringAsync();
            var documento = JsonDocument.Parse(json);
            var root = documento.RootElement;

            // Para compra uso ask porque seria lo que cuesta comprar.
            // Para venta uso bid porque seria lo que nos pagan.
            var propiedad = action == "sale" ? "bid" : "ask";

            if (!root.TryGetProperty(propiedad, out var precio))
                throw new Exception("No pude leer el precio de CriptoYa");

            return precio.GetDecimal();
        }

        private string PasarCodigoParaCriptoYa(string cryptoCode)
        {
            var codigo = cryptoCode.ToLower().Trim();

            if (codigo == "bitcoin")
                return "btc";

            if (codigo == "ethereum")
                return "eth";

            return codigo;
        }
    }
}
