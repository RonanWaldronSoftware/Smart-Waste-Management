import React, { useEffect, useState } from "react";
import { GoogleMap, MarkerF, useLoadScript } from "@react-google-maps/api";
import { getGoogleMapsApiKey } from "../../utils/getEnv";

const BinFormMap = ({ pinPosition, handleMapClick }) => {
  const [map, setMap] = useState(null);
  const [mapPosition, setMapPosition] = useState({
    lat: 58,
    lng: 18,
  });

  useEffect(() => {
    if (pinPosition?.lat && pinPosition?.lng) {
      setMapPosition({
        lat: pinPosition?.lat,
        lng: pinPosition?.lng,
      });
    }
  }, [pinPosition]);
  const mapSettings = {
    size: {
      width: "100%",
      height: "100%",
    },
    zoom: 5,
    cotrolOptions: {
      mapTypeControl: false,
      streetViewControl: false,
      fullscreenControl: true,
    },
  };

  //init maps
  const { isLoaded, loadError } = useLoadScript({
    googleMapsApiKey: getGoogleMapsApiKey(),
  });
  const isGoogleMapsLoaded = () =>
    typeof window.google === "object" && typeof window.google.maps === "object";

  if (loadError) return "Error loading maps";
  if (!isLoaded) return "Loading maps";
  if (!isGoogleMapsLoaded()) return "Google Maps API is loading...";

  return (
    <GoogleMap
      center={mapPosition}
      zoom={mapSettings.zoom}
      mapContainerStyle={mapSettings.size}
      options={mapSettings.cotrolOptions}
      onClick={handleMapClick}
    >
      {pinPosition && <MarkerF position={pinPosition} />}
    </GoogleMap>
  );
};

export default BinFormMap;
