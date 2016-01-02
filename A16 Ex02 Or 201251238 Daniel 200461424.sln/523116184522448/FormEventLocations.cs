﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GMap.NET.WindowsForms;
using GMap.NET;
using System.Threading;

namespace _523116184522448
{
    public partial class EventLocationsForm : Form
    {
        private FBUtilities m_utils;
        private GMapOverlay m_MarkersOverlay;

        public FBUtilities FBUtilities 
        {
            set { m_utils = value; }
        }

        public EventLocationsForm()
        {
            InitializeComponent();
        }

        private void loadMap()
        {
            gMapControl.Invoke(new Action(() => gMapControl.MapProvider = GMap.NET.MapProviders.GoogleMapProvider.Instance));
            gMapControl.Invoke(new Action(() => gMapControl.SetPositionByKeywords("dubnov, Tel Aviv, Israel")));
            m_MarkersOverlay = new GMapOverlay("markers");
            foreach (object obj in m_utils.Events)
            {
                if (m_utils.HasLocationEvent(obj))
                {
                    PointLatLng point = getLatLong(m_utils.GetLatLong(obj));
                    GMap.NET.WindowsForms.Markers.GMarkerGoogle marker = new GMap.NET.WindowsForms.Markers.GMarkerGoogle(
                        point,
                        GMap.NET.WindowsForms.Markers.GMarkerGoogleType.red_small);
                    marker.ToolTipText = m_utils.GetEventName(obj);
                    m_MarkersOverlay.Markers.Add(marker);
                }
            }

            gMapControl.Invoke(new Action(() => gMapControl.Overlays.Add(m_MarkersOverlay)));
            gMapControl.Invoke(new Action(() => gMapControl.ZoomAndCenterMarkers(null)));
        }

        private void buttonFetchEvents_Click(object sender, EventArgs e)
        {
            new Thread(() => m_utils.fetchCollectionAsync(listBoxEvents, m_utils.Events, "Name")).Start();
            new Thread(() => loadMap()).Start();
            
        }

        private void listBoxEvents_SelectedIndexChanged(object sender, EventArgs e)
        {       
            if (m_utils.HasLocationEvent(listBoxEvents.SelectedItem))
            {
                gMapControl.Position = getLatLong(m_utils.GetLatLong(listBoxEvents.SelectedItem));
            }
            else
            {
                gMapControl.ZoomAndCenterMarkers(null);
            }
        }

        private PointLatLng getLatLong(PointD i_location)
        {
            PointLatLng coordinates;
            coordinates = new PointLatLng(i_location.X, i_location.Y);

            return coordinates;
        } 
    }
}
