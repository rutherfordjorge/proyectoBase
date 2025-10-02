namespace ProyectoBase.Api.Domain;

/// <summary>
/// Representa un pronóstico meteorológico con información de fecha y temperaturas asociadas.
/// </summary>
public record class WeatherForecast
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="WeatherForecast"/> con los datos del pronóstico.
    /// </summary>
    /// <param name="date">Fecha del pronóstico meteorológico.</param>
    /// <param name="temperatureC">Temperatura esperada expresada en grados Celsius (°C).</param>
    /// <param name="summary">Descripción resumida de las condiciones climáticas previstas.</param>
    public WeatherForecast(DateOnly date, int temperatureC, string? summary)
    {
        Date = date;
        TemperatureC = temperatureC;
        Summary = summary;
    }

    /// <summary>
    /// Fecha del pronóstico meteorológico.
    /// </summary>
    public DateOnly Date { get; init; }

    /// <summary>
    /// Temperatura esperada expresada en grados Celsius (°C).
    /// </summary>
    public int TemperatureC { get; init; }

    /// <summary>
    /// Descripción resumida de las condiciones climáticas previstas para la fecha indicada.
    /// </summary>
    public string? Summary { get; init; }

    /// <summary>
    /// Temperatura equivalente calculada en grados Fahrenheit (°F) a partir de <see cref="TemperatureC"/>.
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
