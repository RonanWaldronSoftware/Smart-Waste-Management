import React from "react";
import { noResultsImg } from "../assets";

const NoResults = () => {
  return (
    <div className="flex items-center justify-center">
      <img src={noResultsImg} alt="" />
    </div>
  );
};

export default NoResults;
