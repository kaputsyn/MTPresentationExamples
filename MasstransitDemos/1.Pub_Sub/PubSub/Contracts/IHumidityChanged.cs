using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface IHumidityChanged : IWeatherChanged
    {
        int NewHumidityPercent { get; }
    }
}
