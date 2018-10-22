using Domotique.Database;
using Domotique.Model;
using Messages.WebMessages;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Domotique.Controllers
{


    [Produces("application/json")]
    [Route("/rest/[controller]")]
    public partial class StatusController : Controller
    {
        IDataRead _dataRead;

        DomotiqueContext _context;

        ILogger<StatusController> _logger;

        public StatusController(IDataRead dataRead, DomotiqueContext context, ILogger<StatusController> logger)
        {
            _dataRead = dataRead;
            _context = context;
            _logger = logger;
        }


        [HttpGet("test")]
        public IActionResult TestContext()
        {
            _logger.LogTrace("Trace");
            _logger.LogDebug("Debug");
            _logger.LogInformation("Info");
            _logger.LogWarning("Warning");
            _logger.LogWarning(_context.Adapter.Count().ToString());
            var rooms = _context.Rooms.Include(tl => tl.TemperatureSchedules).ThenInclude(t => t.Schedule).ThenInclude(schedule => schedule.Periods);
            foreach (var room in rooms)
            {
                room.ComputeCurrentTemperature();
            }

            return Ok(rooms);
        }


        [HttpGet()]
        public IActionResult Get()
        {
            Status status = new Status()
            {
                Rooms = _dataRead.ReadRoomTemperatures(),
                Devices = _dataRead.ReadDevices(),
                Version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>().Version
            };

            return Ok(status);
        }


        public static List<Point> DouglasPeuckerReduction
            (List<Point> Points, double Tolerance)
        {
            if (Points == null || Points.Count < 3)
                return Points;

            int firstPoint = 0;
            int lastPoint = Points.Count - 1;
            List<int> pointIndexsToKeep = new List<int>
            {
                //Add the first and last index to the keepers
                firstPoint,
                lastPoint
            };

            //The first and the last point cannot be the same
            while (Points[firstPoint].Equals(Points[lastPoint]))
            {
                lastPoint--;
            }

            DouglasPeuckerReduction(Points, firstPoint, lastPoint,
            Tolerance, ref pointIndexsToKeep);

            List<Point> returnPoints = new List<Point>();
            pointIndexsToKeep.Sort();
            foreach (int index in pointIndexsToKeep)
            {
                returnPoints.Add(Points[index]);
            }

            return returnPoints;
        }

        /// <summary>
        /// Douglases the peucker reduction.
        /// </summary>
        /// <param name="points">The points.</param>
        /// <param name="firstPoint">The first point.</param>
        /// <param name="lastPoint">The last point.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <param name="pointIndexsToKeep">The point index to keep.</param>

        private static void DouglasPeuckerReduction(List<Point>
            points, int firstPoint, int lastPoint, double tolerance,
            ref List<int> pointIndexsToKeep)
        {
            double maxDistance = 0;
            int indexFarthest = 0;

            for (int index = firstPoint; index < lastPoint; index++)
            {
                double distance = PerpendicularDistance
                    (points[firstPoint], points[lastPoint], points[index]);
                if (distance > maxDistance)
                {
                    maxDistance = distance;
                    indexFarthest = index;
                }
            }

            if (maxDistance > tolerance && indexFarthest != 0)
            {
                //Add the largest point that exceeds the tolerance
                pointIndexsToKeep.Add(indexFarthest);

                DouglasPeuckerReduction(points, firstPoint,
                indexFarthest, tolerance, ref pointIndexsToKeep);
                DouglasPeuckerReduction(points, indexFarthest,
                lastPoint, tolerance, ref pointIndexsToKeep);
            }
        }

        /// <summary>
        /// The distance of a point from a line made from point1 and point2.
        /// </summary>
        /// <param name="pt1">The PT1.</param>
        /// <param name="pt2">The PT2.</param>
        /// <param name="p">The p.</param>
        /// <returns></returns>
        public static double PerpendicularDistance
            (Point Point1, Point Point2, Point Point)
        {
            //Area = |(1/2)(x1y2 + x2y3 + x3y1 - x2y1 - x3y2 - x1y3)|   *Area of triangle
            //Base = v((x1-x2)²+(x1-x2)²)                               *Base of Triangle*
            //Area = .5*Base*H                                          *Solve for height
            //Height = Area/.5/Base

            double area = Math.Abs(.5 * (Point1.X * Point2.Y + Point2.X *
            Point.Y + Point.X * Point1.Y - Point2.X * Point1.Y - Point.X *
            Point2.Y - Point1.X * Point.Y));
            double bottom = Math.Sqrt(Math.Pow(Point1.X - Point2.X, 2) + Math.Pow(Point1.Y - Point2.Y, 2));
            double height = area / bottom * 2;

            return height;

            //Another option
            //Double A = Point.X - Point1.X;
            //Double B = Point.Y - Point1.Y;
            //Double C = Point2.X - Point1.X;
            //Double D = Point2.Y - Point1.Y;

            //Double dot = A * C + B * D;
            //Double len_sq = C * C + D * D;
            //Double param = dot / len_sq;

            //Double xx, yy;

            //if (param < 0)
            //{
            //    xx = Point1.X;
            //    yy = Point1.Y;
            //}
            //else if (param > 1)
            //{
            //    xx = Point2.X;
            //    yy = Point2.Y;
            //}
            //else
            //{
            //    xx = Point1.X + param * C;
            //    yy = Point1.Y + param * D;
            //}

            //Double d = DistanceBetweenOn2DPlane(Point, new Point(xx, yy));
        }
    }
}
