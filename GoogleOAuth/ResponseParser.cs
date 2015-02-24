using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using GoogleOAuth.Models;

namespace GoogleOAuth
{
    public class ResponseParser
    {
        public static IEnumerable<PicasaItem> Parse(string content)
        {
            var albums = new List<PicasaItem>();
            var xmlBytes = Encoding.UTF8.GetBytes(content);
            using (var xmlStream = new MemoryStream(xmlBytes))
            {
                using (var reader = XmlReader.Create(xmlStream))
                {
                    PicasaItem picasaItem = null;
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "entry":
                                    picasaItem = new PicasaItem();
                                    reader.MoveToAttribute("gd:etag");
                                    picasaItem.Etag = reader.Value;
                                    reader.MoveToElement();
                                    break;
                                case "published":
                                    reader.Read();
                                    if (picasaItem != null) picasaItem.Published = DateTime.Parse(reader.Value);
                                    break;
                                case "updated":
                                    reader.Read();
                                    if (picasaItem != null) picasaItem.Updated = DateTime.Parse(reader.Value);
                                    break;
                                case "title":
                                    reader.Read();
                                    if (picasaItem != null) picasaItem.Title = reader.Value;
                                    break;
                                case "gphoto:id":
                                    reader.Read();
                                    if (picasaItem != null) picasaItem.Id = reader.Value;
                                    break;
                                case "gphoto:location":
                                    reader.Read();
                                    if (picasaItem != null) picasaItem.Location = reader.Value;
                                    break;
                                case "gphoto:numphotos":
                                    reader.Read();
                                    if (picasaItem != null) picasaItem.NumPhotos = Int32.Parse(reader.Value);
                                    break;
                                case "media:content":
                                    reader.MoveToAttribute("url");
                                    if (picasaItem != null) picasaItem.ContentUrl = reader.Value;
                                    reader.MoveToElement();
                                    break;
                                case "media:thumbnail":
                                    reader.MoveToAttribute("url");
                                    if (picasaItem != null) picasaItem.CoverUrl = reader.Value;
                                    reader.MoveToElement();
                                    break;
                            }
                        }
                        else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "entry")
                        {
                            albums.Add(picasaItem);
                        }
                    }
                }
            }
            return albums;
        }
    }
}
