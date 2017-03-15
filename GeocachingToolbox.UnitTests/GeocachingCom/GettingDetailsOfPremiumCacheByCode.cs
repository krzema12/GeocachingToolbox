using System;
using System.Collections.Generic;
using GeocachingToolbox.GeocachingCom;
using Machine.Specifications;
using Rhino.Mocks;

namespace GeocachingToolbox.UnitTests.GeocachingCom
{
    [Subject("Getting details of a premium geocache by its code")]
    public class GettingDetailsOfPremiumCacheByCode
    {
        protected static GCClient _gcClient;
        protected static IGCConnector _stubConnector;
        protected static GCGeocache _subject;
        protected static GCGeocache _expectedResult;

        Establish context = () =>
        {
            _stubConnector = MockRepository.GenerateMock<IGCConnector>();
            _gcClient = new GCClient(_stubConnector);
            _stubConnector.Expect(x => x.GetPage("geocache/GC41HTN"))
                .ReturnContentOf(@"GeocachingCom\WebpageContents\PremiumGeocacheDetails.html").Repeat.Once();
            _stubConnector.Expect(
             x => x.GetContent(Arg<string>.Is.Anything, Arg<IDictionary<string, string>>.Is.Anything))
             .ReturnContentOf(@"GeocachingCom\WebpageContents\Dummy.txt");//dummy

            _subject = new GCGeocache
            {
                Code = "GC41HTN"
            };

            _expectedResult = new GCGeocache
            {
                Code = "GC41HTN",
                Name = "Światowy Dzień Toalet 2012 / World Toilet Day 2012",
                Type = GeocacheType.Traditional,
                Size = GeocacheSize.Micro,
                Difficulty = 2.5f,
                Terrain = 1.5f,
                IsPremium = true,
                IsDetailed = true,
                Status = GeocacheStatus.Published,
                DateHidden = new DateTime(2012, 11, 19),
                Owner = new GCUser("abdul65", -1),
                Waypoint = new Location(54.3759333, 018.6164167),
                Description = @"<div style=""text-align:left\;"">
                                    <center><img alt=""z lewej / inside"" src=""data:image/png; base64,77u/"" style=""width:320px;height:240px;text-align:right\;""> <img alt=""z prawej / inside"" src=""data:image/png; base64,77u/"" style=""width:320px;height:240px;""></center>
                                    <br>
                                    <div style=""text-align:center;"">
                                        <img alt=""w środku / inside"" src=""data:image/png; base64,77u/"" style=""210px;height:102px;""><br>
                                        <br>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""3""><b>[PL] <img src=""data:image/png; base64,77u/""></b></font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><br></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">Inspiracją do zainstalowania tego kesza był Światowy Dzień Toalet (ang. World Toilet Day).</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">Gdy dowiedziałem się o nim to najpierw wpadłem w zdumienie, potem śmiałem się z 10 minut.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><br></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">Sprawdziłem w necie i okazało się, że Światowy Dzień Toalet obchodzony jest corocznie 19 listopada, już od 2001 roku.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">Organizuje je Światowa Organizacja Toaletowa, World Toilet Organisation (WTO), która podjęła się prowadzenia walki o podwyższanie standardów ubikacji na świecie ze szczególnym uwzględnieniem szaletów publicznych.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">Temat jest więc bliski każdemu z nas, szczególne w naszych polskich miastach.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><br></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">Założycielem Organizacji jest Jack Sim, który co roku organizował konferencję World Toilet Summit. Konferencje takie odbyły się w 2001 roku w Singapurze, a potem kolejno w Seulu</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">(Korea Południowa–2002), Tajpej (Tajwan–2003), Pekinie (Chiny–2004) oraz Belfaście</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">(Irlandia Płn.–2005).</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><br></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">Zastanawiałem się jak uczcić taki dzień i przypomniałem sobie o zapomnianych toaletach publicznych wzdłuż Al.Zwycięstwa.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">Są to dwa bardzo nietypowe, zbudowane jako małe (dość gustowne) domki drewniane z bali, ze spadzistymi daszkami. Zupełnie inne niż plastikowe toy-toy'e lub inne betonowe szalety bez wyrazu.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">Zacząłem zgłębiać temat i ku mojemu zaskoczeniu okazało się, że informacji na ten temat nie ma. Skontaktowałem się z Gdańskim oddziałem PTTK i dotarłem do przewodnika Pana Andrzeja Młota.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><br></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">Okazuje się, że domki-toalety powstały pomiędzy 1770, a 1800 rokiem zaraz po oddaniu do użytku ""Wielkiej Aleji Lipowej"" czyli dzisiejszej Al.Zwycięstwa.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">Historia Wielkiej Alei łączy się nierozerwalnie z nazwiskiem burmistrza miasta, fizyka i przyrodnika - Daniela Gralatha starszego. On to bowiem, umiłowawszy swoje miasto zapisał w testamencie 100 tysięcy guldenów na aleję lipową, łączącą Gdańsk z Wrzeszczem. Rok po śmierci fundatora rozpoczęły się mozolne prace ziemne, które wykonywali w ramach ćwiczeń żołnierze gdańskiego garnizonu, a nadzór nad nimi powierzono emerytowanemu komendantowi Twierdzy Wisłoujście – kapitanowi Feliksowi Patzerowi. Wytyczona, zgodnie z przebiegiem historycznego szlaku handlowego, ponad dwukilometrowa trasa zaczynała się przy Bramie Oliwskiej (na wysokości dzisiejszego Placu Zebrań Ludowych) i dochodziła do granicy Wrzeszcza (Langfuhr), który w owych czasach był własnością rodziny von Goltz.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">Na zniwelowanym i zmeliorowanym (!) terenie rozpoczęły się, pod czujnym okiem pastora Jenina z kalwińskiego kościoła św. Elżbiety, nasadzenia 1416 lip, przywiezionych drogą morską z Holandii. Ukończoną w 1770 roku reprezentacyjną aleję tworzyły cztery rzędy drzew, o łącznej szerokości trzydzieści metrów, a składała się ze środkowego pasa przeznaczonego dla pojazdów i dwóch pięciometrowych promenad dla pieszych. Mieszkańcy Gdańska od razu ją polubili i z niecierpliwością wyczekiwali pierwszego kwitnienia drzew.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><br></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">Wspomniane domki od początku stanowiły szalety publiczne przy szlaku z Gdańska do Wrzeszcza. Podobno posiadały podjazdy dla dorożek. Obie usytuowane były przy szlaku, i jednocześnie w powstałych tu parkach.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">Jedna znajduje się w Parku Uphagena w dzisiejszym Wrzeszczu, a druga a dzisiejszym Parku Steffensa.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><br></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">Istnieją od prawie 200-stu lat, remontowane i malowane, ale niezmiennie zachowując swoją funkcję.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">Przetrwały niezniszczone nawet II Wojnę Światową.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><br></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">Szkoda, że obecnie zamknięte na ""cztery spusty""...:(</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><br></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""3""><i><b>Kesz typu ""sprytnie zamaskowane mikro"", znajduje nieopodal domku-toalety we wrzeszczańskim Parku Uphagena.</b></i></font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><br></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><br></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""3""><b>[ENG] <img src=""data:image/png; base64,77u/""></b></font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><br></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">The inspiration of this cache was the World Toilet Day.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">When I learned about it, at first I was amazed, but then it made me laugh for nearly 10 minutes.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><br></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">I googled it later and I found out that the World Toilet Day had been celebrated since 2001 on</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">19<sup>th</sup>November every year.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">The World Toilet Organisation created this event to struggle for the improvement of sanitation conditions worldwide with the special attention to public toilets. This affects all of us, especially when we think of public toilets in Polish cities.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><br></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">Jack Sim founded the WTO and organises The World Toilet Summit every year. The first summit was held in 2001 in Singapore and then in Seoul (South Korea, 2002), Taipei (Taiwan, 2003), Beijing (China, 2004) and in Belfast (Northern Ireland, 2005).</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><br></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">I was wondering how to celebrate the World Toilet Day and it reminded me of the old public toilets along Aleja Zwycięstwa.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">Along the avenue, they are located in two small unusual cabins made of wooden beams with a sloping roof. They look completely different than the plastic Toy-Toys or the bland concrete toilets.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">I started to look into the subject, but to my great surprise there was no information available. I contacted the Gdańsk unit of the Polish Tourist and Sightseeing Society (Polskie Towarzystwo Turystyczno-Krajoznawcze, PTTK) and I got to know Mr Andrzej Młota, guide.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><br></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">I learned that the wooden toilet cabins were built between 1770 and 1800, right after having put The “Grand Avenue of Lime” (“Wielka Aleja Lipowa”) into use , today called Aleja Zwycięska.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">The history of the Grand Avenue is inextricably linked with the city mayor, physicist –Daniel Gralath senior. It was him who cherishing the city decided to will 100,000 of guilders for the construction of an avenue of lime between Gdańsk and Wrzeszcz. One year after his death, arduous works began. They were carried out by Gdańsk garrison soldiers who were supervised by a retired commanding officer of the Wisłoujście fortress – captain Feliks Patzer. The route retraced the historic trade path and it ran from Brama Oliwska (the gate was located near today Plac Zebrań Ludowych – People’s Meeting Square ) to Wrzeszcz (Langfuhr) which belonged to the Goltz family at that time.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">The plantation of 1416 linden trees (transported from the Netherlands by sea) started on the levelled and reclaimed (!) lands. The sharp eyes of a Calvinist pastor Jenin from the Saint Elisabeth church controlled all works. In 1770, the formal avenue was finished. It was lined with four rows of linden trees with the total width of 30 m and consisted of one middle lane and two 5 m wide promenades. Gdańsk dwellers liked it straight and they waited with impatience for the first trees blooming.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><br></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">From the very beginning, the wooden cabins served as public toilets along the trail between Gdańsk and Wrzeszcz. Drives for carriages were said to have led up to these cabins. The toilets were both located along the way in parks that were created here.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">One of the cabins is situated in Park Uphagena in Wrzeszcz, and the other one in Park Steffensa.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">The toilets have existed for nearly 200 years. They were renovated and repainted, but they never ceased to serve their purpose.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">They didn’t get destroyed even during the WWII.</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><br></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""2"">It’s pity that their doors are now “locked and bolted”…</font></font></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><br></p>
                                        <p align=""justify"" style=""margin-bottom:0cm;line-height:100%;""><font face=""Apolonia TT""><font size=""3""><i><b>The cache of a “cleverly hidden micro” type is located near the public toilets cabin in Park Uphagena in Wrzeszcz.</b></i></font></font></p>
                                        <br>
                                        <br>
                                    </div>
                                </div>
                                <a href=""http://info.flagcounter.com/ri2B""><img src=""data:image/png; base64,77u/"" alt=""Flag Counter"" border=""0""></a>"
            };
            _gcClient._dateFormat = "dd/MM/yyyy";

        };

        Because of = () =>
            _gcClient.GetGeocacheDetailsAsync(_subject).Await();

        private It should_return_an_object_with_geocache_details = () =>
        {
            _subject.Logs = null; // don't test logs here
            _subject.ShouldEqualRecursively(_expectedResult);
        };
        It should_call_connectors_GetPage_method_with_proper_address = () =>
            _stubConnector.VerifyAllExpectations();
    }
}
