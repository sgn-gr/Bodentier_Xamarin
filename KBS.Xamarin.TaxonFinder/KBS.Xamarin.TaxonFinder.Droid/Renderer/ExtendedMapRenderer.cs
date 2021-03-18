using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using KBS.App.TaxonFinder.CustomRenderers;
using KBS.App.TaxonFinder.Droid.Renderer;
using System;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using Xamarin.Forms.Platform.Android;
using Polygon = Android.Gms.Maps.Model.Polygon;
using Polyline = Android.Gms.Maps.Model.Polyline;

[assembly: ExportRenderer(typeof(ExtendedMap), typeof(ExtendedMapRenderer))]
namespace KBS.App.TaxonFinder.Droid.Renderer
{
    public class ExtendedMapRenderer : MapRenderer, IOnMapReadyCallback
	{
		private bool _mapDrawn;
		private Polygon _polygon;
		private Polyline _polyline;
		private ObservableCollection<Position> _shapeCoordinates;
		private ObservableCollection<Position> _routeCoordinates;

		public ExtendedMapRenderer(Context context) : base(context)
		{
			_shapeCoordinates = new ObservableCollection<Position>();
			_routeCoordinates = new ObservableCollection<Position>();
		}

		protected override void OnMapReady(GoogleMap map)
		{
			if (_mapDrawn) return;
			base.OnMapReady(map);
			map.MapClick += GoogleMap_MapClick;
			map.CameraMoveStarted += GoogleMap_MapPush;
			map.CameraIdle += GoogleMap_MapStopPush;

			DrawElements();
			_mapDrawn = true;
		}
		private void DrawElements()
		{
			if (((ExtendedMap)Element).MapPositionOption != Data.PositionInfo.PositionOption.Pin)
			{
				NativeMap.Clear();
				_shapeCoordinates = ((ExtendedMap)Element).TempShapeCoordinates;
				if (_shapeCoordinates.Count > 0)
					DrawPolygon();
				_routeCoordinates = ((ExtendedMap)Element).TempRouteCoordinates;
				if (_routeCoordinates.Count > 0)
					DrawPolyline();
			}
			else
			{
				_polyline?.Remove();
				_polygon?.Remove();
			}
		}

		private void DrawPolygon()
		{
			PolygonOptions polygonOptions = GetPolygoneRenderer();
			foreach (var position in _shapeCoordinates)
			{
				polygonOptions.Add(new LatLng(position.Latitude, position.Longitude));
			}
			_polygon = NativeMap.AddPolygon(polygonOptions);
		}

		private static PolygonOptions GetPolygoneRenderer()
		{
			var polygonOptions = new PolygonOptions();
			polygonOptions.InvokeFillColor(0x66FF0000);
			polygonOptions.InvokeStrokeColor(0x66FF0000);
			polygonOptions.InvokeStrokeWidth(10.0f);
			return polygonOptions;
		}

		private void DrawPolyline()
		{
			PolylineOptions polylineOptions = GetPolylineRenderer();
			foreach (var position in _routeCoordinates)
			{
				polylineOptions.Add(new LatLng(position.Latitude, position.Longitude));
			}
			_polyline = NativeMap.AddPolyline(polylineOptions);
		}

		private static PolylineOptions GetPolylineRenderer()
		{
			var polylineOptions = new PolylineOptions();
			polylineOptions.InvokeColor(0x66FF0000);
			polylineOptions.InvokeWidth(10.0f);
			return polylineOptions;
		}

		protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
		{
			if (NativeMap != null)
				NativeMap.MapClick -= GoogleMap_MapClick;

			base.OnElementChanged(e);

			Control?.GetMapAsync(this);
		}

		private void GoogleMap_MapClick(object sender, GoogleMap.MapClickEventArgs e)
		{
			((ExtendedMap)Element).OnTap(new Position(e.Point.Latitude, e.Point.Longitude));
			DrawElements();
		}

		private void GoogleMap_MapPush(object sender, GoogleMap.CameraMoveStartedEventArgs e)
		{
			DrawElements();
		}

		private void GoogleMap_MapStopPush(object sender, EventArgs e)
		{
			DrawElements();
		}
	}
}