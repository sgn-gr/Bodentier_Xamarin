using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms.Maps;
using static KBS.App.TaxonFinder.Data.PositionInfo;

namespace KBS.App.TaxonFinder.CustomRenderers
{
    public class ExtendedMap : Map
	{
		public PositionOption MapPositionOption;
		public event EventHandler<TapEventArgs> Tap;
		public ObservableCollection<Position> ShapeCoordinates;
		public ObservableCollection<Position> TempShapeCoordinates
		{
			get
			{
				switch (ShapeCoordinates.Count())
				{
					case 1:
						return new ObservableCollection<Position> {
						new Position(ShapeCoordinates[0].Latitude, ShapeCoordinates[0].Longitude) { },
						new Position(ShapeCoordinates[0].Latitude,ShapeCoordinates[0].Longitude - 0.00002) { },
						new Position(ShapeCoordinates[0].Latitude - 0.00002,ShapeCoordinates[0].Longitude - 0.00002) { }
					};
					case 2:
						return new ObservableCollection<Position> {
						new Position(ShapeCoordinates[0].Latitude, ShapeCoordinates[0].Longitude) { },
						new Position(ShapeCoordinates[1].Latitude, ShapeCoordinates[1].Longitude) { },
						new Position(ShapeCoordinates[0].Latitude - 0.00002,ShapeCoordinates[0].Longitude - 0.00002) { }
					};
					default:
						return ShapeCoordinates;
				}
			}
			set
			{
				ShapeCoordinates = value;
			}
		}
		public ObservableCollection<Position> RouteCoordinates;
		public ObservableCollection<Position> TempRouteCoordinates
		{
			get
			{
				switch (RouteCoordinates.Count())
				{
					case 1:
						return new ObservableCollection<Position> {
						new Position(RouteCoordinates[0].Latitude, RouteCoordinates[0].Longitude) { },
						new Position(RouteCoordinates[0].Latitude - 0.00002,RouteCoordinates[0].Longitude - 0.00002) { },
					};
					default:
						return RouteCoordinates;
				}
			}
			set
			{
				RouteCoordinates = value;
			}
		}

		public ExtendedMap()
		{
			TempShapeCoordinates = new ObservableCollection<Position>();
			ShapeCoordinates = new ObservableCollection<Position>();
			TempRouteCoordinates = new ObservableCollection<Position>();
			RouteCoordinates = new ObservableCollection<Position>();
		}

		public ExtendedMap(MapSpan region) : base(region)
		{
			TempShapeCoordinates = new ObservableCollection<Position>();
			ShapeCoordinates = new ObservableCollection<Position>();
			TempRouteCoordinates = new ObservableCollection<Position>();
			RouteCoordinates = new ObservableCollection<Position>();
		}

		public void OnTap(Position coordinate)
		{
			OnTap(new TapEventArgs { Position = coordinate });
		}

		protected virtual void OnTap(TapEventArgs e)
		{
			if (Tap != null)
			{
				Tap(this, e);
			}
		}
	}
	public class TapEventArgs : EventArgs
	{
		public Position Position { get; set; }
	}
}
