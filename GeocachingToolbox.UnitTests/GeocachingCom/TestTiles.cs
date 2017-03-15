using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using GeocachingToolbox.GeocachingCom;
using GeocachingToolbox.GeocachingCom.MapFetch;
using GeocachingToolbox.UnitTests.GeocachingCom.WebpageContents.GC2JVEH;
using GeocachingToolbox.UnitTests.GeocachingCom.WebpageContents.GC3RMAT;
using Machine.Specifications;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rhino.Mocks;

namespace GeocachingToolbox.UnitTests.GeocachingCom
{
    [Subject("Test Zoom")]
    public class TestTiles
    {
        static readonly Location bottomLeft = new Location(49.3, 8.3);
        static readonly Location topRight = new Location(49.4, 8.4);
        static int zoomLat;
        static int zoomLon;
        Establish context = () =>
        {

        };
        Because of = () =>
        {
            zoomLat = Tile.CalcZoomLat(bottomLeft, topRight);
            zoomLon = Tile.CalcZoomLon(bottomLeft, topRight);

        };

        It should_return_tiles = () =>
        {
            //zoomLat
            Math.Abs(new Tile(bottomLeft, zoomLat).TileY - new Tile(topRight, zoomLat).TileY).ShouldEqual(1);
            Math.Abs(new Tile(bottomLeft, zoomLat + 1).TileY - new Tile(topRight, zoomLat + 1).TileY).ShouldBeGreaterThan(1);

            // zoomLon
            (new Tile(bottomLeft, zoomLon).TileX + 1).ShouldEqual(new Tile(topRight, zoomLon).TileX);
            (new Tile(bottomLeft, zoomLon + 1).TileX + 1).ShouldBeLessThan(new Tile(topRight, zoomLon + 1).TileX);
        };
    }

    [TestClass]
    public class MsTest
    {
        [TestMethod]
        public void TestTilesWithDifferentZoomLevels()
        {
            var tile = new Tile(new Location(52.5077, 13.4651), 14);
            Assert.AreEqual(tile.TileX, 8804);
            Assert.AreEqual(tile.TileY, 5374);
        }

        [TestMethod] //Premium cache
        public void TestGC2JVEH()
        {
            var stubConnector = MockRepository.GenerateMock<IGCConnector>();
            stubConnector.Expect(
                x => x.GetContent(Arg<string>.Is.Anything, Arg<IDictionary<string, string>>.Is.Anything))
                .ReturnContentOf(@"GeocachingCom\WebpageContents\Dummy.txt");//dummy
            GC2JVEH mockedCache = new GC2JVEH();

            GCClient client = new GCClient(stubConnector);
            client._dateFormat = "yyyy-MM-dd";
            GCGeocache parsedCache = new GCGeocache();
            client.ParseFromText(mockedCache.Data, parsedCache).GetAwaiter().GetResult();
            CompareCaches(parsedCache, mockedCache);
        }
        [TestMethod] //Premium cache
        public void TestGC3RMAT()
        {
            var stubConnector = MockRepository.GenerateMock<IGCConnector>();
            stubConnector.Expect(
                x => x.GetContent(Arg<string>.Is.Anything, Arg<IDictionary<string, string>>.Is.Anything))
                .ReturnContentOf(@"GeocachingCom\WebpageContents\Dummy.txt");//dummy
            GC3RMAT mockedCache = new GC3RMAT();

            GCClient client = new GCClient(stubConnector);
            client._dateFormat = "dd/MM/yyyy";
            GCGeocache parsedCache = new GCGeocache();
            client.ParseFromText(mockedCache.Data, parsedCache).GetAwaiter().GetResult();
            CompareCaches(parsedCache, mockedCache);
        }

        public static void CompareCaches(GCGeocache actual, GCGeocache expected)
        {
            Assert.AreEqual(actual.Name, expected.Name, "Name");
            Assert.AreEqual(actual.Terrain, expected.Terrain, "Terrain");
            Assert.AreEqual(actual.Difficulty, expected.Difficulty, "Difficulty");
            Assert.AreEqual(actual.Code, expected.Code, "Code");
            Assert.AreEqual(actual.Size, expected.Size, "Size");

            string actualDescription = actual.Description?.Replace("\r\n", "\n");
            string expectedDescription = expected.Description?.Replace("\r\n", "\n");

            Assert.AreEqual(actualDescription, expectedDescription, "Description"); // don't test img tags
                                                                                    //if (expected.Description != null)
                                                                                    //  Assert.IsTrue(actual.Description.Contains(expected.Description), "Description"); // don't test img tags
            Assert.AreEqual(actual.DateHidden, expected.DateHidden, "DateHidden");
            Assert.AreEqual(actual.IsPremium, expected.IsPremium, "IsPremium");
            Assert.AreEqual(actual.Status, expected.Status, "Status");
            Assert.AreEqual(actual.IsDetailed, expected.IsDetailed, "IsDetailed");
            Assert.AreEqual((double)actual.Waypoint.Latitude, (double)expected.Waypoint.Latitude, 0.0000001, "Waypoint.Latitude");
            Assert.AreEqual((double)actual.Waypoint.Longitude, (double)expected.Waypoint.Longitude, 0.0000001, "Waypoint.Longitude");
        }
    }



}