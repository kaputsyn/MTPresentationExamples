using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface ITemperatureChanged : IWeatherChanged
    {
        int NewTemperatureKelvin { get; }
    }
}
