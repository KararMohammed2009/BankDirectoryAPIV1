using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankDirectoryApi.Domain.ValueObjects
{
 
        public record GeoCoordinate(double Latitude, double Longitude)
        {
            public double Latitude { get; init; } = Latitude;
            public double Longitude { get; init; } = Longitude;
        }
    
}
