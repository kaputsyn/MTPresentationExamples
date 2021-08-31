using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface IWeatherResponse
    {
        double HumidityPercent { get; }
        double TemperatureKelvin { get; }

        DateTime TimeStamp { get; }

        string Location { get; }

    }
}
