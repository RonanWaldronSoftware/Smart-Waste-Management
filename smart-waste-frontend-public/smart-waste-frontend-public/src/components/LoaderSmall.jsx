import React from "react";

const LoaderSmall = () => {
  return (
    <div
      className="spinner-border-xs d-flex mx-auto my-auto"
      style={{ width: "1rem", height: "1rem", minWidth: "1rem" }}
      role="status"
    ></div>
  );
};

export default LoaderSmall;
