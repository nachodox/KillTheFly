using Blazored.LocalStorage;

using KillTheFly.Shared;
using System.Net.Http.Json;

namespace KillTheFly.Client.Helpers { 
    public class ImageHelper
    {
        public static string BannerImage { get; set; } = "banner.png";
        public static string CharacterImage { get; set; } = "fly-swatter-icon.png";
        public static string FoeImage { get; set; } = "fly-icon.png";
        public static string KillImage { get; set; } = "dead-fly-icon.png";
        public static string LogoImage { get; set; } = "icon-192.png";
        static Dictionary<string, string> BackgroundImages = new Dictionary<string, string>();
        static Dictionary<string, string> BackgroundImagesStatic = new Dictionary<string, string>();
        const string DefaultImage = "iVBORw0KGgoAAAANSUhEUgAAABQAAAAUCAIAAAAC64paAAABhWlDQ1BJQ0MgUHJvZmlsZQAAeJx9kT1Iw0AcxV9TpVIqChYRcchQXbRLFXEsVSyChdJWaNXB5NIvaNKQtLg4Cq4FBz8Wqw4uzro6uAqC4AeIq4uToouU+L+k0CLGg+N+vLv3uHsHCM0KU82eKKBqNSMVj4nZ3Kroe4UfwxhEBJMSM/VEejED1/F1Dw9f78I8y/3cn6NfyZsM8IjEUaYbNeIN4tnNms55nzjISpJCfE48ZdAFiR+5Ljv8xrlos8Azg0YmNU8cJBaLXSx3MSsZKvEMcUhRNcoXsg4rnLc4q5U6a9+TvzCQ11bSXKc5hjiWkEASImTUUUYFNYRp1UgxkaL9mIt/1PYnySWTqwxGjgVUoUKy/eB/8LtbszAdcZICMaD3xbI+xgHfLtBqWNb3sWW1TgDvM3CldfzVJjD3SXqjo4WOgIFt4OK6o8l7wOUOMPKkS4ZkS16aQqEAvJ/RN+WAoVvAv+b01t7H6QOQoa6Wb4CDQ2CiSNnrLu/u6+7t3zPt/n4AG5Vy6mfk8LUAAAAdSURBVHicY9x/cycDuYCJbJ2jmkc1j2oe1UwVzQBqIQJ5m7z6ygAAAABJRU5ErkJggg==";
        public static bool ImagesLoaded(string map)
        {
            var hash = $"{map}-10-10";
            return BackgroundImages.ContainsKey(hash);
        }

        public static string GetImageSync(string map, string hash)
        {
            var fullHash = $"{map}-{hash}";
            if (BackgroundImages.ContainsKey(fullHash))
            {
                return BackgroundImages[fullHash];
            }
            return DefaultImage;
        }
        public static async Task<ImageTile?> GetTile(HttpClient httpClient, string map, string hash)
        {
            Console.WriteLine($"looking for image {map}, {hash}");
            try
            {
                Console.WriteLine($"trying to get image {map}, {hash}");
                var tile = await httpClient.GetFromJsonAsync<ImageTile>($"Image/Map/{map}/{hash}");
                return tile;
            }
            catch (Exception)
            {

            }
            return null;
        }

        public static async Task PopulateFromLocalStorageAll(HttpClient httpClient, ILocalStorageService localstorage, string map)
        {
            if (await localstorage.ContainKeyAsync($"{map}-10-10"))
            {
                for (int x = 0; x < 20; x++)
                {
                    for (int y = 0; y < 20; y++)
                    {
                        var hash = $"{map}-{x}-{y}";
                        if (await localstorage.ContainKeyAsync(hash))
                        {
                            BackgroundImages.Add(hash, await localstorage.GetItemAsStringAsync(hash));
                        }
                    }
                }
            }
            else
            {
                var tiles = await httpClient.GetFromJsonAsync<ImageTile[]>($"Image/Map/{map}");
                if(tiles is null)
                {
                    throw new Exception("no tiles in result");
                }
                var curedTiles = tiles.Select(tile => $"{tile.Map}-{tile.LocationX}-{tile.LocationY}|{tile.ImageBase64}").ToArray();
                var stringTiles = string.Join("#", curedTiles);

                //await localstorage.SetItemAsync("TILES", stringTiles);
                foreach (var tile in tiles)
                {
                    var hash = $"{map}-{tile.LocationX}-{tile.LocationY}";
                    //await localstorage.SetItemAsync(hash, tile.ImageBase64);
                    BackgroundImages.Add(hash, tile.ImageBase64);
                }
            }
        }
        public static async Task PopulateFromLocalStorage(HttpClient httpClient, ILocalStorageService localstorage, string map)
        {
            for(int x = 0; x < 20; x++)
            {
                for(int y = 0; y < 20; y++)
                {
                    var hash = $"{x}-{y}";
                    var fullHash = $"{map}-{hash}";
                    if(await localstorage.ContainKeyAsync(fullHash))
                    {
                        BackgroundImages.Add(hash, await localstorage.GetItemAsStringAsync(fullHash));
                        continue;
                    }
                    var tile = await GetTile(httpClient, map, hash);

                    if (tile is not null)
                    {
                        await localstorage.SetItemAsync(fullHash, tile.ImageBase64);
                        BackgroundImages.Add(hash, tile.ImageBase64);
                    }
                    else
                    {
                        if (BackgroundImagesStatic.Count == 0)
                        {
                            Console.WriteLine($"populating images");
                            PopulateImages();
                        }
                        Console.WriteLine($"trying to post image {map}, {hash}");
                        await httpClient.PostAsJsonAsync("Image/Map", new CreateTileDto
                        {
                            ImageBase64 = BackgroundImagesStatic[hash],
                            LocationX = int.Parse(hash.Split("-")[0]),
                            LocationY = int.Parse(hash.Split("-")[1]),
                            Map = map
                        });
                    }
                }
            }
        }
        private static void PopulateImages()
        {
        }
    }
}
