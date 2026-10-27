import React, { useEffect, useState } from "react";
import { AllBinsMap, Loader, MainWrapper } from "../../components";
import { useSelector } from "react-redux";
import { useNavigate, useSearchParams } from "react-router-dom";
import MapFilterBtns from "./components/MapFilterBtns";
import MapAside from "./components/MapAside";
import { getBinsList } from "../../services/units";
import { useTranslation } from "react-i18next";
import { fetchWithRetry } from "../../utils/refreshAccessToken";
import { getPossibleParents } from "../../services/users";
import LoaderSmall from "../../components/LoaderSmall";

const Map = () => {
  const navigate = useNavigate();
  const accessToken = useSelector((state) => state.user.accessToken);
  const configData = useSelector((state) => state.user.configData);
  const [searchParams] = useSearchParams();
  const query = searchParams.get("filter");

  const { t } = useTranslation("translation");

  const [binsData, setBinsData] = useState([]);
  const [filteredBins, setFilteredBins] = useState([]);
  const [owners, setOwners] = useState([]);

  const [loading, setLoading] = useState(true);
  const [refetch, setRefetch] = useState(false);
  const [filterMenuActive, setFilterMenuActive] = useState(false);

  const [selectedMapFilter, setSelectedMapFilter] = useState("fillLevel");
  const [checkboxFilters, setCheckboxFilters] = useState({
    contentTypeIds: [1, 2],
    fullnessStatusIds: [12, 13, 14],
    unitTypesIds: [1, 2],
    unitModelTypeIds: [],
    userIds: [],
  });

  //fetch data
  useEffect(() => {
    fetchOwners();
  }, []);

  //setup filters from config
  useEffect(() => {
    if (configData && owners.length > 0) {
      let initialCheckboxFilters = {
        contentTypeIds: configData.contentTypes.map((item) => item.id),
        fullnessStatusIds: configData.status
          .filter((item) => item.statusCategoryId === 4)
          .map((item) => item.id),
        unitTypesIds: configData.unitTypes.map((item) => item.id),
        unitModelTypeIds: configData.unitModelTypes.map((item) => item.id),
        userIds: owners.map((item) => item.id),
      };

      if (query) {
        if (query === "alerts") {
          setSelectedMapFilter("alerts");
        } else if (query === "mixed") {
          initialCheckboxFilters.contentTypeIds = [1];
        } else if (query === "recyclable") {
          initialCheckboxFilters.contentTypeIds = [2];
        } else if (query.includes("capacity")) {
          initialCheckboxFilters.fullnessStatusIds = [14];
        }
      }

      setCheckboxFilters(initialCheckboxFilters);
    }
  }, [configData, owners, query]);

  //filter data when alerts are selected
  useEffect(() => {
    if (binsData.length > 0) {
      if (selectedMapFilter === "alerts") {
        const filtered = binsData.filter(
          (item) => item.alertTypes.length !== 0
        );
        setFilteredBins(filtered);
      } else {
        setFilteredBins(binsData);
      }
      //setLoading(false);
    } else {
      setFilteredBins([]);
    }
  }, [binsData, selectedMapFilter]);

  //refetch data whenever checkboxes change
  useEffect(() => {
    setRefetch(true);
    fetchData();
  }, [checkboxFilters]);

  const fetchData = async () => {
    console.log("Refetching!");

    try {
      const data = await fetchWithRetry(
        getBinsList,
        accessToken,
        navigate,
        t,
        "",
        1,
        10000,
        "id",
        "asc",
        false,
        checkboxFilters
      );
      if (data.status === 200) {
        setBinsData(data.data.items);
      }
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
      setRefetch(false);
    }
  };
  const fetchOwners = async () => {
    try {
      const data = await fetchWithRetry(
        getPossibleParents,
        accessToken,
        navigate,
        t,
        2
      );
      if (data.status === 200) {
        setOwners(data.data);
        fetchData();
      }
    } catch (error) {
      setLoading(false);
      console.error(error);
    }
  };

  const handleCheckboxChange = (group, value) => {
    setCheckboxFilters((prevCheckboxes) => ({
      ...prevCheckboxes,
      [group]: prevCheckboxes[group].includes(value)
        ? prevCheckboxes[group].filter(
            (selectedValue) => selectedValue !== value
          )
        : [...prevCheckboxes[group], value],
    }));
  };

  const handleFilterChange = (filterValue) => {
    setSelectedMapFilter(filterValue);
  };

  const unselectAllOwners = () => {
    setCheckboxFilters((prev) => ({ ...prev, userIds: [] }));
  };

  if (loading) {
    return (
      <MainWrapper loader={true}>
        <Loader />
      </MainWrapper>
    );
  }

  return (
    <MainWrapper>
      <div className="flex flex-col w-full">
        {/* Header */}
        <div className="flex items-center justify-between bg-white py-3 px-6 w-full h-fit">
          <h1 className="text-2xl font-medium leading-tight text-themeColors-textColor flex items-centerg gap-4">
            <span>{t("map")}</span>
            {refetch && <LoaderSmall />}
          </h1>

          {/* Filters */}
          <MapFilterBtns
            selectedFilter={selectedMapFilter}
            handleFilterChange={handleFilterChange}
          />

          <button
            className="flex items-center gap-3 btn hover:shadow-btnShadow"
            style={{ borderRadius: "100vh" }}
            onClick={() => setFilterMenuActive((prev) => !prev)}
          >
            <svg
              width="24"
              height="24"
              viewBox="0 0 24 24"
              fill="none"
              xmlns="http://www.w3.org/2000/svg"
            >
              <path
                fillRule="evenodd"
                clipRule="evenodd"
                d="M3 6V8H21V6H3ZM10 18H14V16H10V18ZM18 13H6V11H18V13Z"
                fill="black"
                fillOpacity="0.54"
              ></path>
            </svg>
            {t("additionalFilters")}
          </button>
        </div>

        {/* Map container */}
        <div className="relative w-full h-full overflow-x-hidden">
          {/* Map */}
          <AllBinsMap
            binsData={filteredBins}
            selectedFilter={selectedMapFilter}
            selectedCheckboxes={checkboxFilters}
          />

          {/* Filters menu */}
          <MapAside
            filterMenuActive={filterMenuActive}
            configData={configData}
            owners={owners}
            checkboxFilters={checkboxFilters}
            handleCheckboxChange={handleCheckboxChange}
            unselectAllOwners={unselectAllOwners}
          />
        </div>
      </div>
    </MainWrapper>
  );
};

export default Map;
