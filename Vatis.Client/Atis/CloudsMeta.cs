using MetarDecoder;
using System;
using System.Collections.Generic;
using Vatsim.Vatis.Client.Common;

namespace Vatsim.Vatis.Client.Atis
{
    public class CloudsMeta : AtisMeta
    {
        public override void Parse(DecodedMetar metar)
        {
            List<string> tts = new List<string>();
            List<string> acars = new List<string>();

            if (metar.Clouds != null)
            {
                bool cloudPrefixIncluded = false;

                foreach (var cloud in metar.Clouds)
                {
                    var cloudAmount = CloudsAmount.ContainsKey(cloud.Amount.ToString()) ? CloudsAmount[cloud.Amount.ToString()] : "";
                    var cloudType = CloudsType.ContainsKey(cloud.Type.ToString()) ? CloudsType[cloud.Type.ToString()] : "";

                    if (metar.IsInternational)
                    {
                        var temp = "";
                        if (!cloudPrefixIncluded && cloud.Amount != CloudLayer.CloudAmount.NSC && cloud.Amount != CloudLayer.CloudAmount.NCD && cloud.Amount != CloudLayer.CloudAmount.CLR)
                        {
                            temp += "clouds ";
                            cloudPrefixIncluded = true;
                        }
                        temp += cloudAmount;
                        if (cloud.Type != CloudLayer.CloudType.NULL)
                        {
                            temp += string.Join(" ", cloudType);
                        }
                        if (cloud.BaseHeight != null && cloud.BaseHeight.ActualValue > 0)
                        {
                            temp += string.Join(" ", ((int)cloud.BaseHeight.ActualValue).NumbersToWords(), "feet");
                        }
                        tts.Add(temp);
                    }
                    else
                    {
                        if (cloud.Amount == CloudLayer.CloudAmount.CLR)
                        {
                            tts.Add("Sky clear below one two thousand");
                            acars.Add("CLR");
                        }
                        else
                        {
                            if (cloud.Amount == CloudLayer.CloudAmount.FEW)
                            {
                                tts.Add($"few clouds at {Convert.ToInt32(cloud.BaseHeight.ActualValue).NumbersToWords()}");
                            }
                            else
                            {
                                tts.Add($"{(cloud.IsCeiling ? "ceiling " : "")}{(cloud.BaseHeight != null ? Convert.ToInt32(cloud.BaseHeight.ActualValue).NumbersToWords() + " " : "")}{cloudAmount}{(cloud.Type != CloudLayer.CloudType.NULL ? string.Concat(" ", cloudType) : "")}");
                            }

                            acars.Add($"{ cloud.Amount}{ cloud.BaseHeight.ActualValue / 100:000}{ (cloud.Type != CloudLayer.CloudType.NULL ? string.Concat(" ", cloud.Type.ToString()) : "") }");
                        }
                    }
                }
            }

            TextToSpeech = string.Join(", ", tts).Trim(',').Trim(' ');
            Acars = string.Join(" ", acars).TrimEnd(' ');
        }

        public static Dictionary<string, string> CloudsAmount => new Dictionary<string, string>()
        {
            {"FEW", "few "},
            {"SCT", "scattered "},
            {"BKN", "broken "},
            {"OVC", "overcast "},
            {"VV", "vertical visibility "},
            {"NSC", "no significant clouds "},
            {"NCD", "no clouds detected " },
            {"CLR", "sky clear "},
            {"SKC", "sky clear " }
        };

        public static Dictionary<string, string> CloudsType => new Dictionary<string, string>()
        {
            {"CB", "cumulonimbus " },
            {"TCU", "towering cumulus " }
        };
    }
}
