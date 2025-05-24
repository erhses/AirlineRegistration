using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Enum
{
    public enum FlightStatus
    {
        Registering = 1,  // Бүртгэж байна
        Boarding = 2,     // Онгоцонд сууж байна
        Departed = 3,     // Ниссэн
        Delayed = 4,      // Хойшилсон
        Cancelled = 5,    // Цуцалсан
        NotStarted = 6   // Эхлээгүй
    }
}
