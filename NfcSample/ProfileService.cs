using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NfcSample
{
    public class ProfileService
    {
        private static List<Profile> _profiles = new List<Profile>
        {
            new Profile { Id = 1, Name = "Yoda", ImageUri = "ms-appx:///Assets/Yoda.png" },
            new Profile { Id = 2, Name = "Ahsoka Tano", ImageUri = "ms-appx:///Assets/Ahsoka.png" },
            new Profile { Id = 3, Name = "Obi-Wan Kenobi", ImageUri = "ms-appx:///Assets/Obi-Wan.png" },
            new Profile { Id = 4, Name = "Luminara Unduli", ImageUri = "ms-appx:///Assets/Luminara.png" },
            new Profile { Id = 5, Name = "Anakin Skywalker", ImageUri = "ms-appx:///Assets/Anakin.png" },
            new Profile { Id = 6, Name = "Aayla Secura", ImageUri = "ms-appx:///Assets/Aayla.png" }
        };

        public static Profile Get(int id)
        {
            return _profiles.FirstOrDefault(p => p.Id == id);
        }

        public static List<Profile> GetProfiles()
        {
            return _profiles;
        }
    }
}
