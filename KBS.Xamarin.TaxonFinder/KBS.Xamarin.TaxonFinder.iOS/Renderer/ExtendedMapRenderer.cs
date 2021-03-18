using CoreLocation;
using KBS.App.TaxonFinder.CustomRenderers;
using KBS.App.TaxonFinder.iOS.Renderer;
using MapKit;
using ObjCRuntime;
using System.Collections.ObjectModel;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExtendedMap), typeof(ExtendedMapRenderer))]

namespace KBS.App.TaxonFinder.iOS.Renderer
{
    public class ExtendedMapRenderer : MapRenderer
	{
		private readonly UITapGestureRecognizer _tapRecogniser;
		private MKPolygonRenderer _polygonRenderer;
		private MKPolylineRenderer _polylineRenderer;
		private ObservableCollection<Position> _shapeCoordinates;
		private ObservableCollection<Position> _routeCoordinates;

		public ExtendedMapRenderer()
		{
			_tapRecogniser = new UITapGestureRecognizer(OnTap)
			{
				NumberOfTapsRequired = 1,
				NumberOfTouchesRequired = 1
			};
			_shapeCoordinates = new ObservableCollection<Position>();
			_routeCoordinates = new ObservableCollection<Position>();
		}

		private void NativeMap_DidChangeVisibleRegion(object sender, System.EventArgs e)
		{
			DrawElements();
		}

		private void OnTap(UITapGestureRecognizer recognizer)
		{
			var cgPoint = recognizer.LocationInView(Control);
			var location = ((MKMapView)Control).ConvertPoint(cgPoint, Control);
			((ExtendedMap)Element).OnTap(new Position(location.Latitude, location.Longitude));
			DrawElements();
		}

		protected override void OnElementChanged(ElementChangedEventArgs<View> e)
		{
			Control?.RemoveGestureRecognizer(_tapRecogniser);
			base.OnElementChanged(e);
			if (Control != null)
			{
				var nativeMap = Control as MKMapView;
				Control.AddGestureRecognizer(_tapRecogniser);

				nativeMap.DidChangeVisibleRegion += NativeMap_DidChangeVisibleRegion;
			}
			if (e.OldElement != null)
			{
				var nativeMap = Control as MKMapView;
				if (nativeMap != null)
				{
					if (nativeMap.Overlays != null)
						nativeMap.RemoveOverlays(nativeMap.Overlays);
					nativeMap.OverlayRenderer = null;
					_polygonRenderer = null;
					_polylineRenderer = null;
				}
			}
			if (e.NewElement != null)
				DrawElements();
		}
		private void DrawElements()
		{
			if ((ExtendedMap)Element != null)
			{
				if (((ExtendedMap)Element).MapPositionOption != Data.PositionInfo.PositionOption.Pin)
				{
					_shapeCoordinates = ((ExtendedMap)Element).TempShapeCoordinates;
					if (_shapeCoordinates.Count > 0)
						DrawPolygon();
					_routeCoordinates = ((ExtendedMap)Element).TempRouteCoordinates;
					if (_routeCoordinates.Count > 0)
						DrawPolyline();
				}
			}
		}
		private void DrawPolygon()
		{
			var nativeMap = Control as MKMapView;
			nativeMap.OverlayRenderer = GetPolygoneRenderer;
			CLLocationCoordinate2D[] coords = new CLLocationCoordinate2D[_shapeCoordinates.Count];
			int index = 0;
			foreach (var position in _shapeCoordinates)
			{
				coords[index] = new CLLocationCoordinate2D(position.Latitude, position.Longitude);
				index++;
			}
			if (nativeMap.Overlays != null)
				nativeMap.RemoveOverlays(nativeMap.Overlays);
			var polygoneOverlay = MKPolygon.FromCoordinates(coords);
			nativeMap.AddOverlay(polygoneOverlay);
		}
		MKOverlayRenderer GetPolygoneRenderer(MKMapView mapView, IMKOverlay overlayWrapper)
		{
			if (!Equals(overlayWrapper, null))
			{
				var overlay = Runtime.GetNSObject(overlayWrapper.Handle) as IMKOverlay;
				_polygonRenderer = new MKPolygonRenderer(overlay as MKPolygon)
				{
					FillColor = UIColor.Red,
					StrokeColor = UIColor.Red,
					Alpha = 0.4f,
					LineWidth = 5
				};
			}
			return _polygonRenderer;
		}
		private void DrawPolyline()
		{
			var nativeMap = Control as MKMapView;
			nativeMap.OverlayRenderer = GetPolylineRenderer;
			CLLocationCoordinate2D[] coords = new CLLocationCoordinate2D[_routeCoordinates.Count];
			int index = 0;
			foreach (var position in _routeCoordinates)
			{
				coords[index] = new CLLocationCoordinate2D(position.Latitude, position.Longitude);
				index++;
			}
			if (nativeMap.Overlays != null)
				nativeMap.RemoveOverlays(nativeMap.Overlays);
			var polylineOverlay = MKPolyline.FromCoordinates(coords);
			nativeMap.AddOverlay(polylineOverlay);
		}
		MKOverlayRenderer GetPolylineRenderer(MKMapView mapView, IMKOverlay overlayWrapper)
		{
			if (!Equals(overlayWrapper, null))
			{
				var overlay = Runtime.GetNSObject(overlayWrapper.Handle) as IMKOverlay;
				_polylineRenderer = new MKPolylineRenderer(overlay as MKPolyline)
				{
					FillColor = UIColor.Red,
					StrokeColor = UIColor.Red,
					Alpha = 0.4f,
					LineWidth = 5
				};
			}
			return _polylineRenderer;
		}
	}
}