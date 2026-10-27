import React, { useEffect, useState } from "react";
import {
  DirectionsRenderer,
  GoogleMap,
  MarkerF,
  useLoadScript,
} from "@react-google-maps/api";
import { getGoogleMapsApiKey } from "../../utils/getEnv";

import { binIconNew, binIconSelected } from "../../assets";

const RoutesMap = ({ binsData, selectedBins, setSelectedBins, isDisabled }) => {
  const [map, setMap] = useState(null);

  const [mapKey, setMapKey] = useState(0);

  const [mapPosition, setMapPosition] = useState({
    lat: 49.53977980713286,
    lng: 8.071189147244182,
  });
  const mapSettings = {
    size: {
      width: "100%",
      height: "50vh",
    },
    zoom: 3,
    controlOptions: {
      mapTypeControl: false,
      streetViewControl: false,
      fullscreenControl: true,
    },
  };

  const [markersArray, setMarkersArray] = useState([]);
  const [directions, setDirections] = useState(null);
  const [waypoints, setWaypoints] = useState([]);
  const [currentIndex, setCurrentIndex] = useState(selectedBins.length || 0);
  const [filteredBinsData, setFilteredBinsData] = useState([]);

  const { isLoaded, loadError } = useLoadScript({
    googleMapsApiKey: getGoogleMapsApiKey(),
  });

  const isGoogleMapsLoaded = () =>
    typeof window.google === "object" && typeof window.google.maps === "object";

  useEffect(() => {
    if (binsData && binsData.length > 0) {
      refreshMarkers();
    } else {
      setMarkersArray([]);
    }
  }, [binsData]);

  useEffect(() => {
    if (selectedBins.length === 0) {
      setDirections(null);
      setWaypoints([]);
      setCurrentIndex(0);
      setMapPosition({
        lat: 49.53977980713286,
        lng: 8.071189147244182,
      });
      setMapKey((prevKey) => prevKey + 1);
    } else {
      recalculateRoute(selectedBins);
    }
  }, [selectedBins, isGoogleMapsLoaded()]);

  useEffect(() => {
    if (binsData && binsData.length > 0) {
      const filtered = binsData.filter((bin) =>
        selectedBins.some((selected) => selected.unitId === bin.id)
      );
      setFilteredBinsData(filtered);
    }
  }, [selectedBins, binsData]);

  const refreshMarkers = () => {
    const binsArray = binsData.map((bin, index) => ({
      position: {
        lat: bin.latitude,
        lng: bin.longitude,
      },
      icon: binIconNew,
      name: bin.location,
      id: bin.id,
      index,
    }));

    setMarkersArray(
      binsArray.map((binObj) => ({
        position: {
          lat: binObj.position.lat,
          lng: binObj.position.lng,
        },
        icon: binObj.icon,
        map: map,
        title: binObj.name,
        index: binObj.index,
        id: binObj.id,
      }))
    );
  };

  const handleMarkerClick = (index, item) => {
    const binAlreadyExists = selectedBins.some((bin) => bin.unitId === item.id);

    if (binAlreadyExists) {
      const newSelectedBins = selectedBins.filter(
        (bin) => bin.unitId !== item.id
      );
      setSelectedBins(newSelectedBins);
      recalculateRoute(newSelectedBins);
    } else {
      const newIndex = currentIndex + 1;
      calculateAndDisplayRoute(item.id);
      setCurrentIndex(newIndex);

      setSelectedBins((prev) => [
        ...prev,
        {
          unitId: item.id,
          routeIndex: newIndex,
        },
      ]);

      if (selectedBins.length === 0) {
        setMapPosition({
          lat: item.position.lat,
          lng: item.position.lng,
        });
      }
    }
  };

  const calculateAndDisplayRoute = (binId) => {
    const selectedBin = binsData.find((bin) => bin.id === binId);
    const directionsService = new window.google.maps.DirectionsService();
    const waypointsCopy = [...waypoints];
    waypointsCopy.push({
      location: new window.google.maps.LatLng(
        selectedBin.latitude,
        selectedBin.longitude
      ),
      stopover: true,
    });

    const origin =
      waypoints.length > 0
        ? waypoints[waypoints.length - 1].location
        : new window.google.maps.LatLng(
            selectedBin.latitude,
            selectedBin.longitude
          );

    directionsService.route(
      {
        origin: origin,
        destination: new window.google.maps.LatLng(
          selectedBin.latitude,
          selectedBin.longitude
        ),
        waypoints: waypointsCopy,
        travelMode: window.google.maps.TravelMode.WALKING,
      },
      (result, status) => {
        if (status === window.google.maps.DirectionsStatus.OK) {
          setDirections(result);
          setWaypoints(waypointsCopy);
        } else {
          console.error("Directions request failed due to " + status);
        }
      }
    );
  };

  const recalculateRoute = (newSelectedBins) => {
    if (!Array.isArray(newSelectedBins) || newSelectedBins.length === 0) {
      return;
    }

    const waypointsCopy = [];
    if (window.google && isGoogleMapsLoaded()) {
      for (const bin of newSelectedBins) {
        const selectedBin = binsData.find((b) => b.id === bin.unitId);

        if (selectedBin) {
          waypointsCopy.push({
            location: new window.google.maps.LatLng(
              selectedBin.latitude,
              selectedBin.longitude
            ),
            stopover: true,
          });
        }
      }

      if (waypointsCopy.length === 0) {
        return;
      }

      const directionsService = new window.google.maps.DirectionsService();

      directionsService.route(
        {
          origin: waypointsCopy[0].location,
          destination: waypointsCopy[waypointsCopy.length - 1].location,
          waypoints: waypointsCopy.slice(1, -1),
          travelMode: window.google.maps.TravelMode.WALKING,
        },
        (result, status) => {
          if (status === window.google.maps.DirectionsStatus.OK) {
            setDirections(result);
            setWaypoints(waypointsCopy);
          } else {
            console.error("Directions request failed due to " + status);
          }
        }
      );
    }
  };

  const onLoad = (map) => {
    setMap(map);
  };

  if (loadError) return "Error loading maps";
  if (!isLoaded) return "Loading maps";
  if (!isGoogleMapsLoaded()) return "Google Maps API is loading...";

  return (
    <div className="map-container">
      <GoogleMap
        key={mapKey}
        center={mapPosition}
        zoom={mapSettings.zoom}
        mapContainerStyle={mapSettings.size}
        options={mapSettings.controlOptions}
        onLoad={onLoad}
      >
        {markersArray.length > 0 &&
          markersArray.map((item) => {
            return (
              <MarkerF
                key={item.id}
                position={{
                  lat: item.position.lat,
                  lng: item.position.lng,
                }}
                icon={{
                  url: item.icon,
                }}
                options={{
                  optimized: false,
                }}
                onClick={
                  isDisabled
                    ? () => {}
                    : () => handleMarkerClick(item.index, item)
                }
                title={item.title}
              />
            );
          })}
        {selectedBins.length > 0 && directions && (
          <>
            <DirectionsRenderer
              directions={directions}
              options={{
                suppressMarkers: true,
                preserveViewport: true, // Prevent zoom changes
              }}
            />
            {filteredBinsData.map((selectedBin, index) => {
              return (
                <MarkerF
                  key={index}
                  position={{
                    lat: selectedBin.latitude,
                    lng: selectedBin.longitude,
                  }}
                  icon={{
                    url: binIconSelected,
                  }}
                  onClick={
                    isDisabled
                      ? () => {}
                      : () => handleMarkerClick(index, selectedBin)
                  }
                />
              );
            })}
          </>
        )}
      </GoogleMap>
    </div>
  );
};

export default RoutesMap;
