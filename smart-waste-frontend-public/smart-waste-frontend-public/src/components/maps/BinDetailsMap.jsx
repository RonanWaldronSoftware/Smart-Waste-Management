import React, { useEffect, useState } from "react";
import { GoogleMap, useLoadScript } from "@react-google-maps/api";
import { getGoogleMapsApiKey } from "../../utils/getEnv";

import mixedHighIcon from "../../assets/map-markers/mixed-high-xhdpi.png";
import recyclableHighIcon from "../../assets/map-markers/recyclable-high-xhdpi.png";
import mixedMediumIcon from "../../assets/map-markers/mixed-medium-xhdpi.png";
import recyclableMediumIcon from "../../assets/map-markers/recyclable-medium-xhdpi.png";
import mixedLowIcon from "../../assets/map-markers/mixed-low-xhdpi.png";
import recyclableLowIcon from "../../assets/map-markers/recyclable-low-xhdpi.png";
import CustomMarker from "./CustomMarker";

const BinDetailsMap = ({ binLocation, binCapacity, wasteType }) => {
  const [map, setMap] = useState(null);
  const [markersArray, setMarkersArray] = useState([]);
  const [mapPosition, setMapPosition] = useState({
    lat: binLocation.lat,
    lng: binLocation.lng,
  });
  const mapSettings = {
    size: {
      width: "100%",
      height: "100%",
    },
    zoom: 15,
    cotrolOptions: {
      mapTypeControl: false,
      streetViewControl: false,
      fullscreenControl: false,
    },
  };

  //init maps
  const { isLoaded, loadError } = useLoadScript({
    googleMapsApiKey: getGoogleMapsApiKey(),
  });
  const isGoogleMapsLoaded = () =>
    typeof window.google === "object" && typeof window.google.maps === "object";

  //init markers
  useEffect(() => {
    refreshMarkers();
  }, []);

  function getMarkerIconUrl(level, type) {
    switch (level) {
      case "high":
        switch (type) {
          case 1:
            return mixedHighIcon;
          case 2:
            return recyclableHighIcon;
        }
        break;
      case "medium":
        switch (type) {
          case 1:
            return mixedMediumIcon;
          case 2:
            return recyclableMediumIcon;
        }
        break;
      default:
        switch (type) {
          case 1:
            return mixedLowIcon;
          case 2:
            return recyclableLowIcon;
        }
    }
  }

  //handling markers
  const refreshMarkers = () => {
    setMarkersArray([]);

    const markerIcon = getMarkerIconUrl(binCapacity, wasteType);

    const marker = {
      position: {
        lat: binLocation.lat, // Convert to number
        lng: binLocation.lng, // Convert to number
      },
      icon: markerIcon,
      map: map,
    };

    setMarkersArray([marker]);
  };

  if (loadError) return "Error loading maps";
  if (!isLoaded) return "Loading maps";
  if (!isGoogleMapsLoaded()) return "Google Maps API is loading...";

  return (
    <div className="map-container h-48">
      <GoogleMap
        center={mapPosition}
        zoom={mapSettings.zoom}
        mapContainerStyle={mapSettings.size}
        options={mapSettings.cotrolOptions}
      >
        {markersArray.length > 0 &&
          markersArray.map((item, index) => {
            return <CustomMarker key={index} item={item} />;
          })}
      </GoogleMap>
    </div>
  );
};

export default BinDetailsMap;
