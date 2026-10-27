import React, { useEffect, useState } from "react";
import { MarkerF } from "@react-google-maps/api";
import { useNavigate } from "react-router-dom";

const CustomMarker = ({ item }) => {
  const navigate = useNavigate();
  const [marker, setMarker] = useState(null);

  useEffect(() => {
    if (marker) {
      marker.setIcon({
        url: item.icon,
      });
    }
  }, [item.icon, marker]);

  return (
    <MarkerF
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
      onLoad={(marker) => setMarker(marker)}
      onClick={() => navigate(item.url)}
      title={item.title ?? item.name}
    />
  );
};

export default CustomMarker;
