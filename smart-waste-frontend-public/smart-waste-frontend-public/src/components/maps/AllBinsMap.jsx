import React, { useEffect, useState } from "react";
import { GoogleMap, useLoadScript } from "@react-google-maps/api";
import { getGoogleMapsApiKey } from "../../utils/getEnv";

//map icons
import mixedHighIcon from "../../assets/map-markers/mixed-high-xhdpi.png";
import recyclableHighIcon from "../../assets/map-markers/recyclable-high-xhdpi.png";
import mixedMediumIcon from "../../assets/map-markers/mixed-medium-xhdpi.png";
import recyclableMediumIcon from "../../assets/map-markers/recyclable-medium-xhdpi.png";
import mixedLowIcon from "../../assets/map-markers/mixed-low-xhdpi.png";
import recyclableLowIcon from "../../assets/map-markers/recyclable-low-xhdpi.png";
import batteryDeadIcon from "../../assets/map-markers/battery-dead-xhdpi.png";
import batteryLowIcon from "../../assets/map-markers/battery-low-xhdpi.png";
import batteryMediumIcon from "../../assets/map-markers/battery-medium-xhdpi.png";
import batteryFullIcon from "../../assets/map-markers/battery-full-xhdpi.png";
import errorIcon from "../../assets/map-markers/error-xhdpi.png";
import CustomMarker from "./CustomMarker";

const AllBinsMap = ({ binsData, selectedFilter, selectedCheckboxes }) => {
  const [map, setMap] = useState(null);
  const [markersArray, setMarkersArray] = useState([]);
  const [mapPosition, setMapPosition] = useState({
    lat: 49.53977980713286,
    lng: 8.071189147244182,
  });
  const mapSettings = {
    size: {
      width: "100%",
      height: "100vh",
    },
    zoom: 3,
    controlOptions: {
      mapTypeControl: false,
      streetViewControl: false,
      fullscreenControl: false,
    },
  };
  const [resetMarkers, setResetMarkers] = useState(false);

  //init maps
  const { isLoaded, loadError } = useLoadScript({
    googleMapsApiKey: getGoogleMapsApiKey(),
  });
  const isGoogleMapsLoaded = () =>
    typeof window.google === "object" && typeof window.google.maps === "object";

  //setup markers
  useEffect(() => {
    if (binsData.length === 0) {
      // Clear markers if binsData is empty
      setMarkersArray([]);
      return;
    }

    refreshMarkers();
  }, [binsData, selectedFilter, selectedCheckboxes]);

  function getMarkerIconUrl(bin) {
    if (selectedFilter === "fillLevel") {
      const contentType = bin.contentType.id;
      if (bin.fillLevelPercentage >= bin.fullLevel) {
        if (contentType === 1) {
          return mixedHighIcon;
        } else {
          return recyclableHighIcon;
        }
      } else if (
        bin.fillLevelPercentage >= bin.emptyLevel &&
        bin.fillLevelPercentage < bin.fullLevel
      ) {
        if (contentType === 1) {
          return mixedMediumIcon;
        } else {
          return recyclableMediumIcon;
        }
      } else {
        if (contentType === 1) {
          return mixedLowIcon;
        } else {
          return recyclableLowIcon;
        }
      }
    } else if (selectedFilter === "battery") {
      if (!bin.batteryVoltage || bin.batteryVoltage <= 7) {
        return batteryDeadIcon;
      } else if (bin?.batteryVoltage > 7 && bin.batteryVoltage <= 8) {
        return batteryLowIcon;
      } else if (bin?.batteryVoltage > 8 && bin.batteryVoltage <= 10) {
        return batteryMediumIcon;
      } else return batteryFullIcon;
    }
    return errorIcon;
  }

  //handling markers
  const refreshMarkers = () => {
    if (binsData.length > 0) {
      setMarkersArray(
        binsData.map((bin) => ({
          position: {
            lat: bin.latitude,
            lng: bin.longitude,
          },
          icon: getMarkerIconUrl(bin),
          url: `/bins/${bin.id}`,
          name: bin.location,
          id: bin.id,
          map: map,
        }))
      );
    } else {
      setMarkersArray([]);
    }
  };

  if (loadError) return "Error loading maps";
  if (!isLoaded) return "Loading maps";
  if (!isGoogleMapsLoaded()) return "Google Maps API is loading...";

  return (
    <div className="map-container">
      <GoogleMap
        center={mapPosition}
        zoom={mapSettings.zoom}
        mapContainerStyle={mapSettings.size}
        options={mapSettings.controlOptions}
        onLoad={(map) => setMap(map)}
      >
        {binsData.length > 0 ? (
          markersArray.map((item, index) => {
            return <CustomMarker key={item.id} item={item} />;
          })
        ) : (
          <></>
        )}
      </GoogleMap>
    </div>
  );
};

export default AllBinsMap;
