import React, { useEffect, useState } from "react";
import {
  FaArrowDown,
  FaArrowUp,
  FaExclamationTriangle,
  FaSearch,
  FaWrench,
} from "react-icons/fa";
import { useDispatch, useSelector } from "react-redux";
import { Loader, MainWrapper, NoResults, Pagination } from "../../components";
import { Link, useNavigate } from "react-router-dom";
import { getDamages } from "../../services/damage";
import { useTranslation } from "react-i18next";
import { fetchWithRetry } from "../../utils/refreshAccessToken";
import LoaderSmall from "../../components/LoaderSmall";

const Reports = () => {
  const accessToken = useSelector((state) => state.user.accessToken);
  const [loading, setLoading] = useState(true);
  const [refetching, setRefetching] = useState(false);
  const [fetchError, setFetchError] = useState(false);
  const [damageData, setDamageData] = useState(null);
  const [totalResults, setTotalResults] = useState(0);
  const { t } = useTranslation("translation");

  //pagination states
  const [itemsPerPage, setItemsPerPage] = useState(25);
  const [currentPage, setCurrentPage] = useState(1);
  const [startItem, setStartItem] = useState(null);
  const [endItem, setEndItem] = useState(null);

  //search state
  const [searchQuery, setSearchQuery] = useState("");

  //sorting states
  const [sortColumn, setSortColumn] = useState("id");
  const [sortDirection, setSortDirection] = useState("asc");

  const dispatch = useDispatch();
  const navigate = useNavigate();

  //data fetching
  useEffect(() => {
    setRefetching(true);
    fetchData();
  }, [searchQuery, currentPage, itemsPerPage, sortColumn, sortDirection]);

  //pagination update and filtering
  useEffect(() => {
    if (damageData && damageData.length > 0) {
      //calculate start item
      setStartItem((currentPage - 1) * itemsPerPage + 1);
      //calculate end item
      setEndItem(Math.min(currentPage * itemsPerPage, totalResults));
    }
  }, [
    damageData,
    itemsPerPage,
    currentPage,
    searchQuery,
    sortColumn,
    sortDirection,
  ]);

  //function for fetching data
  const fetchData = async () => {
    try {
      const data = await fetchWithRetry(
        getDamages,
        accessToken,
        navigate,
        t,
        searchQuery,
        currentPage,
        itemsPerPage,
        sortColumn,
        sortDirection
      );
      if (data.status === 200) {
        setDamageData(data.data.items);
        setTotalResults(data.data.count);
      } else {
        setFetchError(true);
      }
    } catch (error) {
      console.error(error);
    } finally {
      setRefetching(false);
      setLoading(false);
    }
  };

  //function that handles page change in pagination
  const handlePageChange = (num) => {
    setCurrentPage(Number(num));
  };

  // Function to handle sorting
  const handleSorting = (column) => {
    if (sortColumn === column) {
      // If the same column is clicked, toggle the sorting direction
      setSortDirection((prevDirection) =>
        prevDirection === "asc" ? "desc" : "asc"
      );
    } else {
      // If a different column is clicked, set the new sorting column and direction to ascending
      setSortColumn(column);
      setSortDirection("asc");
    }
  };

  const getImagePath = (base64) => {
    return `data:image/jpeg;base64,${base64}`;
  };

  if (loading) {
    return (
      <MainWrapper loader={true}>
        <Loader />
      </MainWrapper>
    );
  }

  if (fetchError) {
    return (
      <MainWrapper>
        <div className="flex flex-col w-full">
          <NoResults />
        </div>
      </MainWrapper>
    );
  }

  return (
    <MainWrapper>
      <div className="flex flex-col mx-auto w-full">
        {/* Header */}
        <div className="flex items-center justify-between bg-white py-3 px-6 mb-6">
          <h1 className="text-2xl font-medium leading-tight text-themeColors-textColor flex items-center gap-4">
            {t("damage")}
            {refetching && <LoaderSmall />}
          </h1>
        </div>

        {/* Content */}
        <div className="px-6 flex flex-col gap-4">
          <div className="card p-5 flex flex-col gap-4 bg-white">
            {/* Search bar */}
            <div className="flex items-center justify-between">
              <div className="flex items-center gap-3">
                {/* Search input */}
                <div className="relative w-full max-w-[360px]">
                  <FaSearch className="absolute left-4 top-1/2 -translate-y-1/2 opacity-50" />
                  <input
                    type="text"
                    name="search"
                    id="search"
                    placeholder={t("search")}
                    className="h-full w-full max-h-[42px] pl-16 py-3 pr-3 border border-solid border-themeColors-borderColor focus:border-themeColors-accentColor outline-none"
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                  />
                </div>
              </div>

              {/* Select */}

              <div className="flex items-center gap-2">
                <p>{t("show")}</p>
                <select
                  name="itemsCount"
                  id="itemsCount"
                  className="table-select"
                  value={itemsPerPage}
                  onChange={(e) => setItemsPerPage(Number(e.target.value))}
                >
                  <option value="10">10</option>
                  <option value="25">25</option>
                  <option value="50">50</option>
                  <option value="100">100</option>
                </select>
                <p>{t("results")}</p>
              </div>
            </div>

            {/* Table */}
            <div className="table-wrapper overflow-auto" id="binsTable">
              {damageData && damageData.length > 0 ? (
                <table className="table w-full" style={{ minWidth: "1350px" }}>
                  {/* Table header */}
                  <thead>
                    <tr>
                      <th
                        className=" p-3 pr-9 relative text-left w-32"
                        onClick={() => handleSorting("statusId")}
                      >
                        <span className="block w-[60px]">
                          {t("status")}
                          <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                            <FaArrowUp
                              className={
                                sortColumn === "statusId" &&
                                sortDirection === "asc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                            <FaArrowDown
                              className={
                                sortColumn === "statusId" &&
                                sortDirection === "desc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                          </span>
                        </span>
                      </th>
                      <th
                        className="p-3 pr-9 relative text-left"
                        onClick={() => handleSorting("unit.location")}
                      >
                        {t("location")}
                        <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                          <FaArrowUp
                            className={
                              sortColumn === "unit.location" &&
                              sortDirection === "asc"
                                ? "opacity-100"
                                : " opacity-30"
                            }
                          />
                          <FaArrowDown
                            className={
                              sortColumn === "unit.location" &&
                              sortDirection === "desc"
                                ? "opacity-100"
                                : " opacity-30"
                            }
                          />
                        </span>
                      </th>
                      <th className="w-32 p-3 pr-9 relative text-left">
                        <span className="text-left"> {t("image")}</span>
                      </th>
                      <th
                        className=" p-3 pr-9 relative text-left w-40"
                        onClick={() => handleSorting("unit.UnitModelTypeId")}
                      >
                        <span className="grid w-[60px]">
                          {t("model")}
                          <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                            <FaArrowUp
                              className={
                                sortColumn === "unit.UnitModelTypeId" &&
                                sortDirection === "asc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                            <FaArrowDown
                              className={
                                sortColumn === "unit.UnitModelTypeId" &&
                                sortDirection === "desc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                          </span>
                        </span>
                      </th>
                      <th
                        className="p-3 pr-9 relative text-left w-40"
                        onClick={() => handleSorting("damageType")}
                      >
                        <span className="block w-20 whitespace-nowrap">
                          {t("dmgType")}
                          <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                            <FaArrowUp
                              className={
                                sortColumn === "damageType" &&
                                sortDirection === "asc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                            <FaArrowDown
                              className={
                                sortColumn === "damageType" &&
                                sortDirection === "desc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                          </span>
                        </span>
                      </th>
                      <th
                        className="w-24 p-3 pr-9 relative text-left min-w-[200px]"
                        /* onClick={() => handleSorting("user.name")} */
                      >
                        <span className="block w-24">
                          {t("user")}
                          {/*  <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                            <FaArrowUp
                              className={
                                sortColumn === "user.name" &&
                                sortDirection === "asc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                            <FaArrowDown
                              className={
                                sortColumn === "user.name" &&
                                sortDirection === "desc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                          </span> */}
                        </span>
                      </th>
                      <th
                        className="w-24 p-3 pr-9 relative text-left min-w-[200px]"
                        /* onClick={() => handleSorting("assignedUser.name")} */
                      >
                        <span className="block w-24">
                          {t("assignedTo")}
                          {/* <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                            <FaArrowUp
                              className={
                                sortColumn === "assignedUser.name" &&
                                sortDirection === "asc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                            <FaArrowDown
                              className={
                                sortColumn === "assignedUser.name" &&
                                sortDirection === "desc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                          </span> */}
                        </span>
                      </th>
                    </tr>
                  </thead>

                  {/* Table body */}
                  <tbody>
                    {damageData.map((dmg, i) => (
                      <tr key={dmg.id + "-" + i}>
                        <td
                          className={` align-middle text-center text-white p-3 text-lg ${
                            dmg.statusId === 5
                              ? "bg-themeColors-warningColor"
                              : dmg.statusId === 4
                              ? "bg-themeColors-dangerColor"
                              : ""
                          }`}
                        >
                          <span className=" w-full h-16 grid place-items-center mx-auto">
                            {dmg.statusId === 5 ? (
                              <FaWrench />
                            ) : dmg.statusId === 4 ? (
                              <FaExclamationTriangle />
                            ) : (
                              <></>
                            )}
                          </span>
                        </td>
                        <td>
                          <Link
                            to={`/bins/${dmg.unit.id}?report=true`}
                            className=" text-themeColors-accentColor hover:text-themeColors-accentColorDark font-normal"
                          >
                            {dmg.unit.location}
                          </Link>
                        </td>
                        <td>
                          <img src={dmg.image} style={{ maxWidth: "65px" }} />
                        </td>
                        <td>{dmg.unit.modelTypeName}</td>
                        <td>{dmg.damageType.name}</td>

                        <td>{dmg.userName}</td>
                        <td>{dmg.assignedUserName}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              ) : (
                <NoResults />
              )}
            </div>
            <div className="flex flex-col gap-4 my-3">
              <p>
                {t("currentlyShowed")}{" "}
                {damageData?.length === 0 ? 0 : startItem} {t("to")} {endItem}{" "}
                {t("of")} {totalResults} {t("data")}
              </p>
              {damageData && damageData?.length > 0 && (
                <Pagination
                  totalItems={totalResults}
                  itemsPerPage={itemsPerPage}
                  currentPage={currentPage}
                  setCurrentPage={setCurrentPage}
                  onPageChange={handlePageChange}
                />
              )}
            </div>
          </div>
        </div>
      </div>
    </MainWrapper>
  );
};

export default Reports;
