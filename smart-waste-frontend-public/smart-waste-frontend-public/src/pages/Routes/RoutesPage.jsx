import React, { useEffect, useState } from "react";
import { useSelector } from "react-redux";
import { Link, useNavigate } from "react-router-dom";
import { deleteRoute, getAllRoutes } from "../../services/routes";
import { Loader, MainWrapper, NoResults, Pagination } from "../../components";
import { BiPlus } from "react-icons/bi";
import {
  FaArrowDown,
  FaArrowUp,
  FaEdit,
  FaEye,
  FaSearch,
  FaTrash,
} from "react-icons/fa";
import { useTranslation } from "react-i18next";
import { toast } from "react-toastify";
import { fetchWithRetry } from "../../utils/refreshAccessToken";
import DeleteModal from "../../components/DeleteModal";
import LoaderSmall from "../../components/LoaderSmall";

const RoutesPage = () => {
  const accessToken = localStorage.getItem("smartWasteAccessToken");
  const userData = useSelector((state) => state.user.userData);

  const navigate = useNavigate();
  const { t } = useTranslation("translation");

  const [loading, setLoading] = useState(true);
  const [refetch, setRefetch] = useState(false);
  const [fetchError, setFetchError] = useState(false);
  const [routesData, setRoutesData] = useState(null);
  const [ownersData, setOwnersData] = useState(null);

  //pagination states
  const [itemsPerPage, setItemsPerPage] = useState(25);
  const [currentPage, setCurrentPage] = useState(1);
  const [startItem, setStartItem] = useState(null);
  const [endItem, setEndItem] = useState(null);
  const [totalResults, setTotalResults] = useState(0);

  //search state
  const [searchQuery, setSearchQuery] = useState("");

  //sorting states
  const [sortColumn, setSortColumn] = useState("id");
  const [sortDirection, setSortDirection] = useState("asc");

  //delete modal
  const [deleteModalActive, setDeleteModalActive] = useState(false);
  const [deleteId, setDeleteId] = useState(null);
  const [deleteSubmitting, setDeleteSubmitting] = useState(false);

  //data fetching
  useEffect(() => {
    setRefetch(true);
    fetchData();
  }, [searchQuery, currentPage, itemsPerPage, sortColumn, sortDirection]);

  //pagination update and filtering
  useEffect(() => {
    if (routesData && routesData.length > 0) {
      setStartItem((currentPage - 1) * itemsPerPage + 1);
      //calculate end item
      setEndItem(Math.min(currentPage * itemsPerPage, totalResults));
    }
  }, [routesData]);

  //function for fetching data
  const fetchData = async () => {
    try {
      const data = await fetchWithRetry(
        getAllRoutes,
        accessToken,
        navigate,
        t,
        searchQuery,
        currentPage,
        itemsPerPage,
        sortColumn,
        sortDirection
      );
      if (data?.status === 200) {
        setRoutesData(data.data.items);
        setTotalResults(data.data.count);
      } else {
        setFetchError(true);
        setLoading(false);
      }
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
      setRefetch(false);
    }
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

  //function that handles page change in pagination
  const handlePageChange = (pageNum) => {
    setCurrentPage(Number(pageNum));
  };

  const handleDeleteModal = (routeId) => {
    if (deleteModalActive) {
      setDeleteModalActive(false);
      setDeleteId(null);
    } else {
      setDeleteModalActive(true);
      setDeleteId(routeId);
    }
  };

  //function for deleting selected route
  const handleRouteDelete = async () => {
    setDeleteSubmitting(true);
    try {
      const res = await fetchWithRetry(
        deleteRoute,
        accessToken,
        navigate,
        t,
        deleteId
      );

      if (res.status !== 200) {
        toast.error(t("requestError"));
        handleDeleteModal();
      } else {
        toast.success(t("routeDeleted"));
        handleDeleteModal();
        fetchData();
      }
    } catch (error) {
      toast.error(t("requestError"));
      console.error(error);
    } finally {
      setDeleteSubmitting(false);
    }
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
          <h1 className="text-2xl font-medium leading-tight text-themeColors-textColor flex items-centerg gap-4">
            {t("routes")}
            {refetch && <LoaderSmall />}
          </h1>

          {/* Additional filters */}
          <div className="flex items-center gap-2">
            {userData?.role?.id < 5 ? (
              <Link
                to="/routes/add"
                className="btn btn-primary flex items-center"
              >
                <BiPlus />
                <span>{t("add")}</span>
              </Link>
            ) : (
              <></>
            )}
          </div>
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
              {routesData && routesData.length > 0 ? (
                <table className="table w-full" style={{ minWidth: "700px" }}>
                  {/* Table header */}
                  <thead>
                    <tr>
                      {/*  <th className="p-3">
                    <span className="grid w-10 place-items-center">
                      <input
                        type="checkbox"
                        className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
                        checked={
                          selectedItems.length === sortedUsersData.length
                        }
                        onChange={handleSelectAll}
                      />
                    </span>
                  </th> */}
                      <th
                        className=" p-3 pr-9 relative text-left"
                        onClick={() => handleSorting("name")}
                      >
                        <span className="block w-[60px]">
                          {t("name")}
                          <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                            <FaArrowUp
                              className={
                                sortColumn === "name" && sortDirection === "asc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                            <FaArrowDown
                              className={
                                sortColumn === "name" &&
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
                        onClick={() => handleSorting("user.name")}
                      >
                        {t("customer")}
                        <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
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
                        </span>
                      </th>
                      <th className="w-24 p-3 pr-9 relative text-left">
                        <span className="text-left"> {t("units")}</span>
                        {/*  <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                          <FaArrowUp
                            className={
                              sortColumn === "unitsCount" &&
                              sortDirection === "asc"
                                ? "opacity-100"
                                : " opacity-30"
                            }
                          />
                          <FaArrowDown
                            className={
                              sortColumn === "unitsCount" &&
                              sortDirection === "desc"
                                ? "opacity-100"
                                : " opacity-30"
                            }
                          />
                        </span> */}
                      </th>
                      {/* <th
                        className=" p-3 pr-9 relative text-left"
                        onClick={() => handleSorting("email")}
                      >
                        <span className="grid w-[60px]">
                          {t("email")}
                          <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                            <FaArrowUp
                              className={
                                sortColumn === "email" &&
                                sortDirection === "asc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                            <FaArrowDown
                              className={
                                sortColumn === "email" &&
                                sortDirection === "desc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                          </span>
                        </span>
                      </th>
                      <th
                        className="w-24 p-3 pr-9 relative text-left"
                        onClick={() => handleSorting("status")}
                      >
                        <span className="block w-20">
                          {t("status")}
                          <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                            <FaArrowUp
                              className={
                                sortColumn === "status" &&
                                sortDirection === "asc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                            <FaArrowDown
                              className={
                                sortColumn === "status" &&
                                sortDirection === "desc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                          </span>
                        </span>
                      </th>
                      <th
                        className="w-24 p-3 pr-9 relative text-left"
                        onClick={() => handleSorting("country")}
                      >
                        <span className="block w-24">
                          {t("country")}
                          <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                            <FaArrowUp
                              className={
                                sortColumn === "country" &&
                                sortDirection === "asc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                            <FaArrowDown
                              className={
                                sortColumn === "country" &&
                                sortDirection === "desc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                          </span>
                        </span>
                      </th> */}

                      <th className="w-[7.5rem] p-3 relative">
                        {/* Options */}
                      </th>
                    </tr>
                  </thead>

                  {/* Table body */}
                  <tbody>
                    {routesData.map((route) => (
                      <tr key={route.id}>
                        {/* <td>
                          <span className="grid w-10 place-items-center">
                            <input
                              type="checkbox"
                              className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
                              checked={selectedItems.includes(user.id)}
                              onChange={() => handleSelectItem(user.id)}
                            />
                          </span>
                        </td> */}
                        <td className="align-middle p-3 ">{route.name}</td>
                        <td>{route.ownerName}</td>
                        <td>{route.unitCount}</td>
                        {/* <td className="align-middle p-3 ">{user.email}</td>
                        <td>{user.status}</td>
                        <td>{user.country}</td> */}
                        <td>
                          {userData?.role?.id < 5 ? (
                            <div className="flex gap-2">
                              <Link
                                to={`/routes/${route.id}`}
                                className="btn bg-themeColors-bgLightColor text-themeColors-textColorLight hover:bg-themeColors-accentColor hover:text-white aspect-square grid place-items-center"
                              >
                                <FaEdit />
                              </Link>
                              <button
                                onClick={() => handleDeleteModal(route.id)}
                                className="btn bg-themeColors-bgLightColor text-themeColors-textColorLight hover:bg-themeColors-accentColor hover:text-white aspect-square grid place-items-center"
                              >
                                <FaTrash />
                              </button>
                            </div>
                          ) : (
                            <div className="flex gap-2">
                              <Link
                                to={`/routes/${route.id}`}
                                className="btn bg-themeColors-bgLightColor text-themeColors-textColorLight hover:bg-themeColors-accentColor hover:text-white aspect-square grid place-items-center"
                              >
                                <FaEye />
                              </Link>
                            </div>
                          )}
                        </td>
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
                {t("currentlyShowed")} {startItem} {t("to")} {endItem} {t("of")}{" "}
                {totalResults} {t("data")}
              </p>
              {routesData && routesData.length > 0 && (
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

        {deleteModalActive && (
          <DeleteModal
            handleCancel={() => handleDeleteModal()}
            handleConfirm={handleRouteDelete}
            submitting={deleteSubmitting}
          />
        )}
      </div>
    </MainWrapper>
  );
};

export default RoutesPage;
