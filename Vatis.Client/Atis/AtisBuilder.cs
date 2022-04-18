using MetarDecoder;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Vatsim.Vatis.Client.AudioForVatsim;
using Vatsim.Vatis.Client.Common;
using Vatsim.Vatis.Client.Config;
using Vatsim.Vatis.Client.Core;
using Vatsim.Vatis.Client.TextToSpeech;

namespace Vatsim.Vatis.Client.Atis
{
    public class AtisBuilder : IAtisBuilder 
    {
        private readonly IAppConfig mAppConfig;
        private readonly INavaidDatabase mNavData;
        private readonly ITextToSpeechRequest mTextToSpeechRequest;
        private readonly IAudioManager mAudioManager;
        private Airport mAirport;

        public AtisBuilder(IAppConfig appConfig, INavaidDatabase airportDatabase, ITextToSpeechRequest textToSpeechRequest, IAudioManager audioManager)
        {
            mAppConfig = appConfig;
            mNavData = airportDatabase;
            mTextToSpeechRequest = textToSpeechRequest;
            mAudioManager = audioManager;
        }

        public async Task BuildAutomaticAtisAsync(AtisComposite composite, CancellationToken token)
        {
            if (composite == null)
            {
                throw new Exception("Composite is null");
            }

            if (composite.CurrentPreset == null)
            {
                throw new Exception("CurrentPreset is null");
            }

            if (composite.DecodedMetar == null)
            {
                throw new Exception("DecodedMetar is null");
            }

            mAirport = mNavData.GetAirport(composite.Identifier);
            if (mAirport == null)
            {
                throw new Exception($"{composite.Identifier} not found in airport database.");
            }

            StringBuilder atisString = new StringBuilder();

            var metar = composite.DecodedMetar;

            var time = DoParse(metar, new ObservationTimeMeta(composite));
            var surfaceWind = DoParse(metar, new SurfaceWindMeta(composite));
            var rvr = DoParse(metar, new RunwayVisualRangeMeta());
            var visibility = DoParse(metar, new VisibilityMeta());
            var presentWeather = DoParse(metar, new PresentWeatherMeta());
            var clouds = DoParse(metar, new CloudsMeta());
            var temp = DoParse(metar, new TemperatureMeta());
            var dew = DoParse(metar, new DewpointMeta());
            var pressure = DoParse(metar, new PressureMeta());

            var notams = "";
            if (!string.IsNullOrEmpty(composite.CurrentPreset.Notams) || composite.NotamDefinitions.Any(t => t.Enabled))
            {
                notams = metar.IsInternational ? "Notices to airmen. " : "Notices to air missions. ";
                notams += string.Join(" ", new[] { composite.CurrentPreset.Notams, string.Join(". ", composite.NotamDefinitions.Where(t => t.Enabled).Select(t => t.Text)) });
            }

            var airportConditions = "";
            if (!string.IsNullOrEmpty(composite.CurrentPreset.AirportConditions) || composite.AirportConditionDefinitions.Any(t => t.Enabled))
            {
                airportConditions = string.Join(" ", new[] { composite.CurrentPreset.AirportConditions, string.Join(". ", composite.AirportConditionDefinitions.Where(t => t.Enabled).Select(t => t.Text)) });
            }

            var atisLetter = char.Parse(composite.CurrentAtisLetter).LetterToPhonetic();

            atisString.Append(composite.CurrentPreset.Template);
            atisString.Replace("[FACILITY]", mAirport.Name);
            atisString.Replace("[ATIS_LETTER]", atisLetter);
            atisString.Replace("[ATIS_CODE]", atisLetter);
            atisString.Replace("[TIME]", time.TextToSpeech);
            atisString.Replace("[OBS_TIME]", time.TextToSpeech);
            atisString.Replace("[WIND]", surfaceWind.TextToSpeech);
            atisString.Replace("[SURFACE_WIND]", surfaceWind.TextToSpeech);
            atisString.Replace("[RVR]", rvr.TextToSpeech);
            atisString.Replace("[VIS]", visibility.TextToSpeech);
            atisString.Replace("[PREVAILING_VISIBILITY]", visibility.TextToSpeech);
            atisString.Replace("[PRESENT_WX]", presentWeather.TextToSpeech);
            atisString.Replace("[PRESENT_WEATHER]", presentWeather.TextToSpeech);
            atisString.Replace("[CLOUDS]", clouds.TextToSpeech);
            atisString.Replace("[TEMP]", temp.TextToSpeech);
            atisString.Replace("[DEW]", dew.TextToSpeech);
            atisString.Replace("[PRESSURE]", pressure.TextToSpeech);
            atisString.Replace("[WX]", $"{surfaceWind.TextToSpeech} {visibility.TextToSpeech} {rvr.TextToSpeech} {presentWeather.TextToSpeech} {clouds.TextToSpeech} {temp.TextToSpeech} {dew.TextToSpeech} {pressure.TextToSpeech}");
            atisString.Replace("[FULL_WX_STRING]", $"{surfaceWind.TextToSpeech} {visibility.TextToSpeech} {rvr.TextToSpeech} {presentWeather.TextToSpeech} {clouds.TextToSpeech} {temp.TextToSpeech} {dew.TextToSpeech} {pressure.TextToSpeech}");
            atisString.Replace("[ARPT_COND]", airportConditions.Trim(' '));
            atisString.Replace("[NOTAMS]", notams.Trim(' '));

            if (metar.IsInternational)
            {
                if (composite.TransitionLevels != null)
                {
                    var transitionLevel = composite.TransitionLevels.FirstOrDefault(t => metar.Pressure.ActualValue >= t.Low && metar.Pressure.ActualValue <= t.High);

                    if (transitionLevel != null)
                    {
                        atisString.Replace("[TL]", "Transition level flight level " + transitionLevel.Altitude.NumberToSingular());
                    }
                    else
                    {
                        atisString.Replace("[TL]", "Transition level not determined");
                    }
                }
                else
                {
                    atisString.Replace("[TL]", "Transition level not determined");
                }
            }

            if (!metar.IsInternational)
            {
                atisString.Append($"ADVISE ON INITIAL CONTACT, YOU HAVE INFORMATION {atisLetter}.");
            }

            if (!metar.IsInternational)
            {
                List<string> acarsWxString = new()
                {
                    surfaceWind.Acars,
                    visibility.Acars,
                    rvr.Acars,
                    presentWeather.Acars,
                    clouds.Acars,
                    $"{temp.Acars}/{dew.Acars}",
                    pressure.Acars
                };

                var acarsNotams = "";
                if (!string.IsNullOrEmpty(composite.CurrentPreset.Notams) || composite.NotamDefinitions.Any(t => t.Enabled))
                {
                    acarsNotams = "NOTAMS... " + string.Concat(composite.CurrentPreset.Notams, string.Join(". ", composite.NotamDefinitions.Where(t => t.Enabled).Select(t => t.Text)));
                }

                StringBuilder acarsText = new StringBuilder();
                acarsText.Append(composite.CurrentPreset.Template);
                acarsText.Replace("[FACILITY]", mAirport.ID);
                acarsText.Replace("[ATIS_LETTER]", composite.CurrentAtisLetter);
                acarsText.Replace("[ATIS_CODE]", composite.CurrentAtisLetter);
                acarsText.Replace("[TIME]", time.Acars);
                acarsText.Replace("[OBS_TIME]", time.Acars);
                acarsText.Replace("[WIND]", surfaceWind.Acars);
                acarsText.Replace("[SURFACE_WIND]", surfaceWind.Acars);
                acarsText.Replace("[RVR]", rvr.Acars);
                acarsText.Replace("[VIS]", visibility.Acars);
                acarsText.Replace("[PREVAILING_VISIBILITY]", visibility.Acars);
                acarsText.Replace("[PRESENT_WX]", presentWeather.Acars);
                acarsText.Replace("[PRESENT_WEATHER]", presentWeather.Acars);
                acarsText.Replace("[CLOUDS]", clouds.Acars);
                acarsText.Replace("[TEMP]", temp.Acars);
                acarsText.Replace("[DEW]", dew.Acars);
                acarsText.Replace("[PRESSURE]", pressure.Acars);
                acarsText.Replace("[WX]",
                    string.Join(" ", acarsWxString.Where(t => !string.IsNullOrEmpty(t))));
                acarsText.Replace("[FULL_WX_STRING]",
                    string.Join(" ", acarsWxString.Where(t => !string.IsNullOrEmpty(t))));
                acarsText.Replace("[ARPT_COND]", airportConditions);
                acarsText.Replace("[NOTAMS]", acarsNotams);
                acarsText.Append($" ...ADVS YOU HAVE INFO { composite.CurrentAtisLetter }.");

                var str = acarsText.ToString();
                str = Regex.Replace(str, @"\s+", " ");
                composite.AcarsText = str.Replace("*", "")
                    .Replace("+", "")
                    .ToUpper();
            }
            else
            {
                var str = atisString.ToString();
                str = Regex.Replace(str, @"(\.+)(\w+)", "$2");
                str = Regex.Replace(str, @"(\,+)(\w+)", "$2");
                str = Regex.Replace(str, @"\s\.", ".");
                str = Regex.Replace(str, @"\s\,", ",");
                str = Regex.Replace(str, @"\s+", " ");
                str = Regex.Replace(str, @"\.+", ".");
                str = Regex.Replace(str, @"\&", "and");
                str = Regex.Replace(str, @"\*", "");
                composite.AcarsText = str.Replace("*", "")
                    .Replace("+", "")
                    .ToUpper();
            }

            var tts = FormatForTextToSpeech(atisString.ToString().ToUpper(), composite);

            try
            {
                await Task.Run(async () =>
                {
                    await Task.Delay(5000, token);
                    var response = mTextToSpeechRequest.RequestSynthesizedText(tts, token);
                    if (response.Result != null)
                    {
                        await mAudioManager.AddOrUpdateBot(response.Result, composite.AtisCallsign, composite.AfvFrequency, mAirport.Latitude, mAirport.Longitude);

                        PostIdsUpdate(composite);
                    }
                }, token);
            }
            catch (OperationCanceledException) { }
        }

        public async Task BuildManualAtisAsync(AtisComposite composite, byte[] memoryStream, CancellationToken token)
        {
            if (composite == null)
            {
                throw new Exception("Composite is null");
            }

            if (composite.CurrentPreset == null)
            {
                throw new Exception("CurrentPreset is null");
            }

            if (composite.DecodedMetar == null)
            {
                throw new Exception("DecodedMetar is null");
            }

            mAirport = mNavData.GetAirport(composite.Identifier);
            if (mAirport == null)
            {
                throw new Exception($"{composite.Identifier} not found in airport database.");
            }

            StringBuilder atisString = new StringBuilder();

            var metar = composite.DecodedMetar;

            var time = DoParse(metar, new ObservationTimeMeta(composite));
            var surfaceWind = DoParse(metar, new SurfaceWindMeta(composite));
            var rvr = DoParse(metar, new RunwayVisualRangeMeta());
            var visibility = DoParse(metar, new VisibilityMeta());
            var presentWeather = DoParse(metar, new PresentWeatherMeta());
            var clouds = DoParse(metar, new CloudsMeta());
            var temp = DoParse(metar, new TemperatureMeta());
            var dew = DoParse(metar, new DewpointMeta());
            var pressure = DoParse(metar, new PressureMeta());

            var notams = "";
            var airportConditions = "";

            if (!string.IsNullOrEmpty(composite.CurrentPreset.AirportConditions))
            {
                airportConditions += composite.CurrentPreset.AirportConditions;
            }

            if (!string.IsNullOrEmpty(composite.CurrentPreset.Notams))
            {
                notams = metar.IsInternational ? "Notices to airmen. " : "Notices to air missions. ";
                notams += composite.CurrentPreset.Notams;
            }

            var atisLetter = char.Parse(composite.CurrentAtisLetter).LetterToPhonetic();

            atisString.Append(composite.CurrentPreset.Template);
            atisString.Replace("[FACILITY]", mAirport.Name);
            atisString.Replace("[ATIS_LETTER]", atisLetter);
            atisString.Replace("[ATIS_CODE]", atisLetter);
            atisString.Replace("[TIME]", time.TextToSpeech);
            atisString.Replace("[OBS_TIME]", time.TextToSpeech);
            atisString.Replace("[WIND]", surfaceWind.TextToSpeech);
            atisString.Replace("[SURFACE_WIND]", surfaceWind.TextToSpeech);
            atisString.Replace("[RVR]", rvr.TextToSpeech);
            atisString.Replace("[VIS]", visibility.TextToSpeech);
            atisString.Replace("[PREVAILING_VISIBILITY]", visibility.TextToSpeech);
            atisString.Replace("[PRESENT_WX]", presentWeather.TextToSpeech);
            atisString.Replace("[PRESENT_WEATHER]", presentWeather.TextToSpeech);
            atisString.Replace("[CLOUDS]", clouds.TextToSpeech);
            atisString.Replace("[TEMP]", temp.TextToSpeech);
            atisString.Replace("[DEW]", dew.TextToSpeech);
            atisString.Replace("[PRESSURE]", pressure.TextToSpeech);
            atisString.Replace("[WX]", $"{surfaceWind.TextToSpeech} {visibility.TextToSpeech} {rvr.TextToSpeech} {presentWeather.TextToSpeech} {clouds.TextToSpeech} {temp.TextToSpeech} {dew.TextToSpeech} {pressure.TextToSpeech}");
            atisString.Replace("[FULL_WX_STRING]", $"{surfaceWind.TextToSpeech} {visibility.TextToSpeech} {rvr.TextToSpeech} {presentWeather.TextToSpeech} {clouds.TextToSpeech} {temp.TextToSpeech} {dew.TextToSpeech} {pressure.TextToSpeech}");
            atisString.Replace("[ARPT_COND]", airportConditions);
            atisString.Replace("[NOTAMS]", notams);

            if (metar.ICAO.StartsWith("K") || metar.ICAO.StartsWith("P"))
            {
                atisString.Append($"ADVISE ON INITIAL CONTACT, YOU HAVE INFORMATION {atisLetter}.");
            }

            composite.AcarsText = atisString.ToString().Replace("*", "").Replace("+", "").ToUpper();

            try
            {
                if (memoryStream != null)
                {
                    await mAudioManager.AddOrUpdateBot(memoryStream, composite.AtisCallsign, composite.AfvFrequency, mAirport.Latitude, mAirport.Longitude);

                    PostIdsUpdate(composite);
                }
            }
            catch (OperationCanceledException) { }
        }

        private string FormatForTextToSpeech(string input, AtisComposite composite)
        {
            // parse zulu times
            input = Regex.Replace(input, @"([0-9])([0-9])([0-9])([0-8])Z",
                m => string.Format($"{ int.Parse(m.Groups[1].Value).NumberToSingular() } { int.Parse(m.Groups[2].Value).NumberToSingular() } { int.Parse(m.Groups[3].Value).NumberToSingular() } { int.Parse(m.Groups[4].Value).NumberToSingular() } zulu"));

            // format vhf frequencies
            input = Regex.Replace(input, @"(1\d\d\.\d\d?\d?)", m => Convert.ToDouble(m.Groups[1].Value).DecimalToWordString(composite.DecodedMetar.IsInternational));

            // format numbers to singluar words
            input = Regex.Replace(input, @"\*(-?[\,0-9]+)", m => int.Parse(m.Groups[1].Value.Replace(",", "")).NumbersToWordsGroup());

            // letters
            input = Regex.Replace(input, @"\*([A-Z]{1,2}[0-9]{0,2})", m => string.Format("{0}", m.Value.ConvertAlphaNumericToWordGroup())).Trim();

            // parse taxiways
            input = Regex.Replace(input, @"\bTWY ([A-Z]{1,2}[0-9]{0,2})\b", m => $"TWY { m.Groups[1].Value.ConvertAlphaNumericToWordGroup() }");
            input = Regex.Replace(input, @"\bTWYS ([A-Z]{1,2}[0-9]{0,2})\b", m => $"TWYS { m.Groups[1].Value.ConvertAlphaNumericToWordGroup() }");

            // parse runways
            input = Regex.Replace(input, @"\b(RY|RWY|RWYS)?\s?([0-9]{1,2})([LRC]?)\b", m => StringUtils.RwyNumbersToWords(int.Parse(m.Groups[2].Value), m.Groups[3].Value,
                (!string.IsNullOrEmpty(m.Groups[1].Value) ? true : false),
                (!string.IsNullOrEmpty(m.Groups[1].Value) && m.Groups[1].Value == "RWYS") ? true : false));

            // user defined contractions
            foreach (var x in composite.Contractions)
            {
                input = input.SafeReplace(x.String.ToUpper(), x.Spoken.ToUpper(), true);
            }

            // default contractions
            foreach (var word in Translations)
            {
                input = input.SafeReplace(word.Key.ToUpper(), word.Value.ToUpper(), true);
            }

            // format navaids identifiers
            var navaids = Regex.Matches(input, @"(?<=\+)([A-Z]{3})");
            if (navaids.Count > 0)
            {
                foreach (Match m in navaids)
                {
                    if (m.Success)
                    {
                        try
                        {
                            var find = mNavData.GetNavaid(m.Value);
                            if (find != null)
                            {
                                input = Regex.Replace(input, $@"\b(?<=\+){m.Value}\b", x => find.Name).Replace("+", "");
                            }
                        }
                        catch { }
                    }
                }
            }

            // format airport identifiers
            var airports = Regex.Matches(input, @"(?<=\+)([A-Z0-9]{4})");
            if (airports.Count > 0)
            {
                foreach (Match m in airports)
                {
                    if (m.Success)
                    {
                        try
                        {
                            var find = mNavData.GetAirport(m.Value);
                            if (find != null)
                            {
                                input = Regex.Replace(input, $@"\b(?<=\+){ m.Value }\b", x => find.Name).Replace("+", "");
                            }
                        }
                        catch { }
                    }
                }
            }

            input = Regex.Replace(input, @"(\.+)(\w+)", "$2");
            input = Regex.Replace(input, @"(\,+)(\w+)", "$2");
            input = Regex.Replace(input, @"\s\.", ".");
            input = Regex.Replace(input, @"\s\,", ",");
            input = Regex.Replace(input, @"\s+", " ");
            input = Regex.Replace(input, @"\.+", ".");
            input = Regex.Replace(input, @"\&", "and");
            input = Regex.Replace(input, @"\*", "");

            return input;
        }

        private ParsedData DoParse(DecodedMetar metar, AtisMeta meta)
        {
            meta.Parse(metar);
            return new ParsedData
            {
                Acars = meta.Acars,
                TextToSpeech = $"{meta.TextToSpeech}."
            };
        }

        private async void PostIdsUpdate(AtisComposite composite)
        {
            if (Debugger.IsAttached)
                return;

            if (string.IsNullOrEmpty(composite.IDSEndpoint) || composite.CurrentPreset == null)
                return;

            try
            {
                var client = new RestClient(composite.IDSEndpoint)
                {
                    Timeout = 5000
                };
                var request = new RestRequest
                {
                    Method = Method.POST
                };
                var json = new IdsRestRequest
                {
                    Facility = composite.Identifier,
                    Preset = composite.CurrentPreset.Name,
                    AtisLetter = composite.CurrentAtisLetter,
                    AirportConditions = composite.CurrentPreset.AirportConditions,
                    Notams = composite.CurrentPreset.Notams,
                    Timestamp = DateTime.UtcNow
                };
                request.AddParameter("application/json", JsonConvert.SerializeObject(json), ParameterType.RequestBody);
                await client.ExecuteAsync(request);
            }
            catch { }
        }

        private Dictionary<string, string> Translations => new Dictionary<string, string>
        {
            {"ACFT", "AIRCRAFT"},
            {"ADVS", "ADVISE"},
            {"ADVSD", "ADVISED"},
            {"ADVZY", "ADVISORY"},
            {"ADVZYS", "ADVISORIES"},
            {"ALS", "APPROACH LIGHTING SYSTEM"},
            {"ALT", "ALTITUDE"},
            {"ALTS", "ALTITUDES"},
            {"APCH", "APPROACH"},
            {"APCHS", "APPROACHES"},
            {"APP", "APPROACH"},
            {"APPR", "APPROACH"},
            {"APPRS", "APPROACHES"},
            {"APPS", "APPROACHES"},
            {"ARPT", "AIRPORT"},
            {"ARPTS", "AIRPORTS"},
            {"ARR", "ARRIVAL"},
            {"ARRS", "ARRIVALS"},
            {"ATTN", "ATTENTION"},
            {"AUTH", "AUTHORIZED"},
            {"AVBL", "AVAILABLE"},
            {"BA", "BRAKING ACTION"},
            {"BAA", "BRAKING ACTION ADVISORIES"},
            {"BC", "BACKCOURSE"},
            {"BTWN", "BETWEEN"},
            {"CAUT", "CAUTION"},
            {"CLNC", "CLEARANCE"},
            {"CLR", "CLEAR"},
            {"CLRD", "CLEARED"},
            {"CLSD", "CLOSED"},
            {"CMSN", "COMMISSION"},
            {"CMSND", "COMMISSIONED"},
            {"CTC", "CONTACT"},
            {"CTL", "CONTROL"},
            {"CTLD", "CONTROLLED"},
            {"CD", "CLEARANCE DELIVERY"},
            {"DCMSN", "DE-COMISSIONED"},
            {"DCMSND", "DE-COMISSIONED"},
            {"DEP", "DEPARTURE"},
            {"DEPS", "DEPARTURES"},
            {"DEPTS", "DEPARTURES"},
            {"DEPTG", "DEPARTING"},
            {"DIST", "DISTANCE"},
            {"DRCTN", "DIRECTION"},
            {"DURG", "DURING"},
            {"DURN", "DURATION"},
            {"EFCT", "EFFECT"},
            {"EXPC", "EXPECT"},
            {"EFF", "EFFECTIVE"},
            {"EQPT", "EQUIPMENT"},
            {"EXPT", "EXPECT"},
            {"FLT", "FLIGHT"},
            {"FREQ", "FREQUENCY"},
            {"FT", "FEET"},
            {"GND", "GROUND"},
            {"GS", "GLIDE-SLOPE"},
            {"HDG", "HEADING"},
            {"HDGS", "HEADINGS"},
            {"HELI", "HELICOPTER"},
            {"HS", "HOLD SHORT"},
            {"HAZD WX INFO", "HAZARDOUS WEATHER INFORMATION"},
            {"INBD", "INBOUND"},
            {"INTXN", "INTERSECTION"},
            {"INST", "INSTRUMENT"},
            {"INVOF", "IN VICINITY OF"},
            {"LAHSO", "LAND AND HOLD SHORT OPERATIONS"},
            {"LDG", "LANDING"},
            {"LGT", "LIGHT"},
            {"LGTD", "LIGHTED"},
            {"LGTS", "LIGHTS"},
            {"LLWS", "LOW LEVEL WIND SHEAR"},
            {"LLZ", "LOCALIZER"},
            {"LOC", "LOCALIZER"},
            {"MOD", "MODERATE"},
            {"MULTI", "MULTIPLE"},
            {"NA", "NOT AUTHORIZED"},
            {"NOTAM", "NO-TAM"},
            {"NOTAMS", "NO-TAMS"},
            {"OPER", "OPERATE"},
            {"OPS", "OPERATIONS"},
            {"OTS", "OUT OF SERVICE"},
            {"OUBD", "OUTBOUND"},
            {"PIREP", "PILOT WEATHER REPORT"},
            {"PROC", "PROCEDURE"},
            {"PROG", "PROGRESS"},
            {"RMNG", "REMAINING"},
            {"RMVL", "REMOVAL"},
            {"RQST", "REQUEST"},
            {"RQSTD", "REQUESTED"},
            {"RWY", "RUNWAY"},
            {"RWYS", "RUNWAYS"},
            {"SIMUL", "SIMULTANEOUS"},
            {"SVC", "SERVICE"},
            {"SVCS", "SERVICES"},
            {"SVR", "SEVERE"},
            {"TFC", "TRAFFIC"},
            {"TFR", "TEMPORARY FLIGHT RESTRICTION"},
            {"TURB", "TURBULENCE"},
            {"TWY", "TAXIWAY"},
            {"TWYS", "TAXIWAYS"},
            {"US", "UNSERVICEABLE"},
            {"UNCTLD", "UNCONTROLLED"},
            {"UNUSBL", "UNUSABLE"},
            {"USBL", "USABLE"},
            {"VFY", "VERIFY"},
            {"VCTR", "VECTOR"},
            {"VCTRS", "VECTORS"},
            {"VIS", "VISUAL"},
            {"XPDR", "TRANSPONDER"},
            {"XPDRS", "TRANSPONDERS"},
            {"XPNDR", "TRANSPONDER"},
            {"XPNDRS", "TRANSPONDERS"},
            {"XTM", "EXTREME"},
            {"N", "NORTH"},
            {"NNE", "NORTH NORTHEAST"},
            {"NE", "NORTHEAST"},
            {"ENE", "EAST NORTHEAST"},
            {"E", "EAST"},
            {"ESE", "EAST SOUTHEAST"},
            {"SE", "SOUTHEAST"},
            {"SSE", "SOUTH SOUTHEAST"},
            {"S", "SOUTH"},
            {"SSW", "SOUTH SOUTHWEST"},
            {"SW", "SOUTHWEST"},
            {"WSW", "WEST SOUTHWEST"},
            {"W", "WEST"},
            {"WNW", "WEST NORTHWEST"},
            {"NW", "NORTHWEST"},
            {"NNW", "NORTH NORTHWEST"},
            {"READBACK", "REEDBACK"},
            {"READBACKS", "REEDBACKS"},
            {"ATIS", "ATE-IS"},
            {"RNAV", "AREA NAVIGATION"},
            {"INFO", "INFORMATION"},
            {"WIND", "WEND"},
            {"CALLSIGN", "CALL-SIGN"},
            {"ILS", "EYE EL ESS"},
            {"DEPG", "DEPARTING"},
            {"ARVNG", "ARRIVING"},
            {"VOR", "V-O-R"},
            {"HIWAS", "HIGH-WAZ"},
            {"FM", "FROM"},
            {"RY", "RUNWAY"},
            {"WX", "WEATHER"},
            {"FSS", "FLIGHT SERVICE STATION"},
            {"HAZD", "HAZARDOUS"},
            {"WPT", "WAYPOINT"},
            {"BTN", "BETWEEN" },
            {"TWR", "TOWER" },
            {"ADZ", "ADVISE" },
            {"MSL", "MEAN SEA LEVEL" },
            {"RLS", "RELEASE" },
            {"LNDG", "LANDING" },
            {"NUM", "NUMEROUS" },
            {"OAT", "OUTSIDE AIR TEMPERATURE" },
            {"RPTD", "REPORTED" },
            {"PAPI", "PRECISION APPROACH PATH INDICATOR" },
            {"NM", "NAUTICAL MILE" },
            {"LND", "LAND" },
            {"SERV", "SERVICE" },
            {"BIV", "BIRD ACTIVITY VICINITY" },
            {"AVLB", "AVAILABLE" },
            {"VCNTY", "VICINITY" },
            {"ACKMTS", "ACKNOWLEDGE-MENTS" },
            {"APRT", "AIRPORT" },
            {"RDBKS", "READBACKS" },
            {"SHRT", "SHORT" },
            {"EXP", "EXPECT" },
            {"GC", "GROUND CONTROL" },
            {"CLSGN", "CALL-SIGN" },
            {"VA", "VISUAL APPROACH" },
            {"INCL", "INCLUDE" },
            {"TRNSPNDR", "TRANSPONDER" },
            {"RQRD", "REQUIRED" },
            {"ACTVT", "ACTIVATE" },
            {"TYS", "TAXIWAYS" },
            {"INT", "INTERSECTION" },
            {"MI", "MILE" },
            {"UFN", "UNTIL FURTHER NOTICE" },
            {"SFC", "SURFACE" },
            {"TODA", "TAKE-OFF DISTANCE" },
            {"VC", "VICINITY" },
            {"INSTR", "INSTRUMENT" },
            {"CNTD", "CONDUCTED" },
            {"INTL", "INTERNATIONAL" },
            {"ASDEX", "AS-DEX" },
            {"ASDE-X", "AS-DEX" }
        };
    }

    [Serializable]
    internal class IdsRestRequest
    {
        public string Facility { get; set; }
        public string Preset { get; set; }
        public string AtisLetter { get; set; }
        public string AirportConditions { get; set; }
        public string Notams { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
