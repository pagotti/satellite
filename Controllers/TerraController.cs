using Microsoft.AspNetCore.Mvc;

namespace Terra.Controllers;

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{    

    private async Task<byte[]> GetImage(Region region)
    {
        var sat = RegionTranslate(region);
        using (var cli = new HttpClient())
        {
            return await cli.GetByteArrayAsync($"https://cdn.star.nesdis.noaa.gov/{sat}/ABI/FD/GEOCOLOR/678x678.jpg?t=${DateTime.Now}");             
        }
    }

    private string RegionTranslate(Region region)
    {
        return region switch
        {
            Region.West => "GOES17",
            _ => "GOES16"
        };
    }

    private string RegionDescription(Region region)
    {
        return region switch
        {
            Region.West => "GOES-WEST - Full Disk",
            _ => "GOES-EAST - Full Disk",
        };
    }

    
    [HttpGet("Jpg/{region?}", Name = "GetJpg")]
    [Produces("image/jpg")]
    public async Task<IActionResult> GetJpg(Region region = Region.East)
    {
        return File(await GetImage(region), "image/jpg");
    }

    [HttpGet("Data/{region?}", Name = "GetString")]
    [Produces("application/json")]
    public async Task<ImageData> GetString(Region region = Region.East)
    {
        var image = await GetImage(region);

        return new ImageData(
            DateTime.Now,             
            "GOES Image Views - NASA STAR",
            RegionDescription(region),
            "GeoColor",
            678, 
            678,
            $"data:image/jpg;base64,{Convert.ToBase64String(image)}"
        );
        
    }

}
