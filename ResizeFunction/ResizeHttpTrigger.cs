using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using System.Linq;

namespace Cs.Resize
{
    public static class ResizeHttpTrigger
    {
        [FunctionName("ResizeHttpTrigger")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            try
            {
                // Récupérer les paramètres w et h
                string wParam = req.Query["w"];
                string hParam = req.Query["h"];
                if (string.IsNullOrWhiteSpace(wParam) || string.IsNullOrWhiteSpace(hParam))
                {
                    return new BadRequestObjectResult("Les paramètres 'w' (largeur) et 'h' (hauteur) sont obligatoires.");
                }
                if (!int.TryParse(wParam, out int width) || width <= 0 || !int.TryParse(hParam, out int height) || height <= 0)
                {
                    return new BadRequestObjectResult("Les paramètres 'w' et 'h' doivent être des entiers strictement positifs.");
                }

                // Vérifier que le corps n'est pas vide
                if (req.ContentLength == null || req.ContentLength == 0)
                {
                    return new BadRequestObjectResult("Le corps de la requête doit contenir une image.");
                }

                using (var ms = new MemoryStream())
                {
                    await req.Body.CopyToAsync(ms);
                    ms.Position = 0;
                    try
                    {
                        using (Image image = Image.Load(ms))
                        {
                            image.Mutate(x => x.Resize(width, height));
                            using (var outStream = new MemoryStream())
                            {
                                image.Save(outStream, new JpegEncoder());
                                byte[] targetImageBytes = outStream.ToArray();
                                return new FileContentResult(targetImageBytes, "image/jpeg");
                            }
                        }
                    }
                    catch (SixLabors.ImageSharp.UnknownImageFormatException)
                    {
                        return new BadRequestObjectResult("Le corps de la requête n'est pas une image valide ou le format n'est pas supporté.");
                    }
                    catch (Exception ex)
                    {
                        log.LogWarning($"Erreur lors du traitement de l'image : {ex.Message}");
                        return new BadRequestObjectResult("Erreur lors du traitement de l'image. Vérifiez le format et la validité de l'image envoyée.");
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogError($"Erreur inattendue : {ex.Message}");
                return new BadRequestObjectResult("Erreur inattendue lors du traitement de la requête. Veuillez vérifier vos paramètres et votre image.");
            }
        }
    }
}
