import React from "react";

const Loader = () => {
  return (
    <div
      className="spinner-border d-flex mx-auto my-auto"
      style={{ width: "3rem", height: "3rem" }}
      role="status"
    ></div>
  );
};

export default Loader;
