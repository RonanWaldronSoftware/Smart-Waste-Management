import React, { useEffect, useState } from "react";
import { BiPlus, BiWindowOpen } from "react-icons/bi";
import {
  FaArrowDown,
  FaArrowUp,
  FaBatteryQuarter,
  FaDoorOpen,
  FaEdit,
  FaExclamationCircle,
  FaFileExport,
  FaLock,
  FaNewspaper,
  FaRecycle,
  FaSearch,
  FaTrash,
  FaTrashAlt,
  FaTrashRestoreAlt,
} from "react-icons/fa";
import { useSelector } from "react-redux";
import { Loader, MainWrapper, NoResults, Pagination } from "../../components";
import { Link, useNavigate } from "react-router-dom";
import { deleteBin, getBinsList, setUnitStatus } from "../../services/units";
import { toast } from "react-toastify";
import ExportModal from "../../components/ExportModal";
import { useTranslation } from "react-i18next";
import { fetchWithRetry } from "../../utils/refreshAccessToken";
import DeleteModal from "../../components/DeleteModal";
import { formatDate } from "../../utils/formatDate";
import LoaderSmall from "../../components/LoaderSmall";
import {
  AppliancesIcon,
  BooksIcon,
  DiscIcon,
  FruitIcon,
  GlassIcon,
  HouseholdIcon,
  LiquidIcon,
  PlasticIcon,
  PrinterIcon,
  SprayIcon,
  TextileIcon,
  CrockeryIcon,
  BatteryIcon,
  CanIcon,
} from "../../assets";
import BinDamaged from "../../assets/icons/BinDamaged";

const BinsList = () => {
  const accessToken = useSelector((state) => state.user.accessToken);
  const userData = useSelector((state) => state.user.userData);
  const navigate = useNavigate();
  const { t } = useTranslation("translation");

  const [loading, setLoading] = useState(true);
  const [refetch, setRefetch] = useState(false);
  const [submittingStatus, setSubmittingStatus] = useState(null);
  const [binsData, setBinsData] = useState(null);
  const [totalResults, setTotalResults] = useState(0);
  const [fetchAllBins, setFetchAllBins] = useState(false);

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

  //checkbox selection
  const [selectedItems, setSelectedItems] = useState([]);

  //export
  const [exportModalActive, setExportModalActive] = useState(false);
  const [exportData, setExportData] = useState([]);

  //delete modal
  const [deleteModalActive, setDeleteModalActive] = useState(false);
  const [deleteId, setDeleteId] = useState(null);
  const [deleteSubmitting, setDeleteSubmitting] = useState(false);

  //data fetching
  useEffect(() => {
    setRefetch(true);
    fetchData();

    // Fetch data every 1 minute
    const intervalId = setInterval(() => {
      setRefetch(true);
      fetchData();
    }, 60000);

    // Clear the interval when the component unmounts
    return () => clearInterval(intervalId);
  }, [
    fetchAllBins,
    searchQuery,
    currentPage,
    itemsPerPage,
    sortColumn,
    sortDirection,
  ]);

  //pagination update and filtering
  useEffect(() => {
    if (binsData && binsData.length > 0) {
      setStartItem((currentPage - 1) * itemsPerPage + 1);
      //calculate end item
      setEndItem(Math.min(currentPage * itemsPerPage, totalResults));
    }
  }, [binsData]);

  //function for fetching data
  const fetchData = async () => {
    try {
      const data = await fetchWithRetry(
        getBinsList,
        accessToken,
        navigate,
        t,
        searchQuery,
        currentPage,
        itemsPerPage,
        sortColumn,
        sortDirection,
        fetchAllBins
      );
      if (data.status === 200) {
        // Perform the logout and redirect
        setBinsData(data.data.items);
        setTotalResults(data.data.count);
      }
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
      setRefetch(false);
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

  //bin actiavtion
  const handleBinStatus = async (binId) => {
    setSubmittingStatus(binId);
    try {
      const res = await fetchWithRetry(
        setUnitStatus,
        accessToken,
        navigate,
        t,
        binId
      );
      if (res.status === 200) {
        toast.success(t("unitSuccessfullyUpdated"));
        fetchData();
      }
    } catch (error) {
      toast.error(t("requestError"));
      console.error(error);
    } finally {
      setSubmittingStatus(null);
    }
  };

  //delete bin
  const handleDeleteModal = (binId) => {
    if (deleteModalActive) {
      setDeleteModalActive(false);
      setDeleteId(null);
    } else {
      setDeleteModalActive(true);
      setDeleteId(binId);
    }
  };
  const handleDelete = async () => {
    setDeleteSubmitting(true);
    try {
      const res = await fetchWithRetry(
        deleteBin,
        accessToken,
        navigate,
        t,
        deleteId
      );
      if (res.status === 200) {
        toast.success(t("binDeleted"));
        setDeleteSubmitting(false);
        setDeleteModalActive(false);
        fetchData();
      } else {
        setDeleteSubmitting(false);
        setDeleteModalActive(false);
      }
    } catch (error) {
      toast.error(t("requestError"));
      console.error(error);
    }

    fetchData();
  };

  //handling checkbox selection
  const handleSelectAll = () => {
    if (binsData.every((bin) => selectedItems.includes(bin.id))) {
      // If all items are already selected, unselect all.
      const filtered = selectedItems.filter(
        (item) => !binsData.some((bin) => bin.id === item)
      );
      setSelectedItems(filtered);
    } else {
      // Select all items on the current page.
      const newItems = binsData.map((item) => item.id);
      setSelectedItems((prev) => [...prev, ...newItems]);
    }
  };
  const handleSelectItem = (itemId) => {
    if (selectedItems.includes(itemId)) {
      // Item is already selected, so unselect it.
      setSelectedItems(selectedItems.filter((id) => id !== itemId));
    } else {
      // Item is not selected, so select it.
      setSelectedItems([...selectedItems, itemId]);
    }
  };

  //handle export modal
  const handleExportModal = () => {
    if (exportModalActive) {
      setExportModalActive(false);
      setExportData([]);
    } else {
      const uniqueSelectedItems = Array.from(new Set(selectedItems));
      setExportData(uniqueSelectedItems);
      setExportModalActive(true);
    }
  };
  const resetExportData = () => {
    setSelectedItems([]);
    setExportData([]);
    setExportModalActive(false);
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
      <div className="flex flex-col mx-auto w-full">
        {/* Header */}
        <div className="flex items-center justify-between bg-white py-3 px-6 mb-6">
          <h1 className="text-2xl font-medium leading-tight text-themeColors-textColor flex items-center gap-4">
            {t("bins")}
            {refetch && <LoaderSmall />}
          </h1>

          {/* Additional filters */}
          {userData?.role?.id === 1 && (
            <div className="flex items-center gap-2">
              <Link to="/bins/create-bulk-bin" className="btn btn-secondary">
                <span>{t("createBinBulk")}</span>
              </Link>
              <Link
                to="/bins/create-bin"
                className="btn btn-primary flex items-center"
              >
                <BiPlus />
                <span>{t("addBin")}</span>
              </Link>
            </div>
          )}
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
                {/* all bins checkbox*/}
                <label className="relative inline-flex items-center cursor-pointer gap-2">
                  <input
                    type="checkbox"
                    className="sr-only peer"
                    checked={fetchAllBins}
                    onChange={() => setFetchAllBins((prev) => !prev)}
                  />
                  <div className="w-11 h-6 bg-white border border-themeColors-bgColor peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-themeColors-bgColor after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-themeColors-accentColor peer-checked:border-themeColors-accentColor peer-checked:after:bg-white"></div>
                  <span className=" whitespace-nowrap">{t("allUnits")}</span>
                </label>
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
              {binsData && binsData.length > 0 ? (
                <table className="table w-full" style={{ minWidth: "1450px" }}>
                  {/* Table header */}
                  <thead>
                    <tr>
                      <th className="p-3 w-16">
                        <span className="grid w-10 place-items-center">
                          <input
                            type="checkbox"
                            className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
                            name="selectAllRows"
                            id="selectAllRows"
                            checked={binsData.every((bin) =>
                              selectedItems.includes(bin.id)
                            )}
                            onChange={handleSelectAll}
                          />
                        </span>
                      </th>
                      <th
                        className=" p-3 pr-9 relative text-left w-32"
                        onClick={() => handleSorting("ContentTypeId")}
                      >
                        <span className="block">
                          {t("wasteType")}
                          <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                            <FaArrowUp
                              className={
                                sortColumn === "ContentTypeId" &&
                                sortDirection === "asc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                            <FaArrowDown
                              className={
                                sortColumn === "ContentTypeId" &&
                                sortDirection === "desc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                          </span>
                        </span>
                      </th>
                      <th
                        className="p-3 pr-9 relative text-left w-[200px]"
                        onClick={() => handleSorting("location")}
                      >
                        {t("location")}
                        <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                          <FaArrowUp
                            className={
                              sortColumn === "location" &&
                              sortDirection === "asc"
                                ? "opacity-100"
                                : " opacity-30"
                            }
                          />
                          <FaArrowDown
                            className={
                              sortColumn === "location" &&
                              sortDirection === "desc"
                                ? "opacity-100"
                                : " opacity-30"
                            }
                          />
                        </span>
                      </th>
                      <th
                        className="p-3 pr-9 relative text-left w-[200px]"
                        onClick={() => handleSorting("serialNumber")}
                      >
                        <span className="text-left"> {t("serialNumber")}</span>
                        <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                          <FaArrowUp
                            className={
                              sortColumn === "serialNumber" &&
                              sortDirection === "asc"
                                ? "opacity-100"
                                : " opacity-30"
                            }
                          />
                          <FaArrowDown
                            className={
                              sortColumn === "serialNumber" &&
                              sortDirection === "desc"
                                ? "opacity-100"
                                : " opacity-30"
                            }
                          />
                        </span>
                      </th>
                      <th
                        className=" p-3 pr-9 relative text-left w-28"
                        onClick={() => handleSorting("binLevel")}
                      >
                        <span className="grid w-[60px]">
                          {t("filled")}
                          <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                            <FaArrowUp
                              className={
                                sortColumn === "binLevel" &&
                                sortDirection === "asc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                            <FaArrowDown
                              className={
                                sortColumn === "binLevel" &&
                                sortDirection === "desc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                          </span>
                        </span>
                      </th>
                      <th
                        className="p-3 pr-9 relative text-left w-28"
                        /* onClick={() => handleSorting("battery")} */
                      >
                        <span className="block w-20">
                          {t("battery")}
                          {/* <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                            <FaArrowUp
                              className={
                                sortColumn === "battery" &&
                                sortDirection === "asc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                            <FaArrowDown
                              className={
                                sortColumn === "battery" &&
                                sortDirection === "desc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                          </span> */}
                        </span>
                      </th>
                      <th
                        className="p-3 pr-9 relative text-left w-36"
                        /* onClick={() => handleSorting("temp")} */
                      >
                        <span className="block w-24">
                          {t("temp")}
                          {/* <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                            <FaArrowUp
                              className={
                                sortColumn === "temp" && sortDirection === "asc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                            <FaArrowDown
                              className={
                                sortColumn === "temp" &&
                                sortDirection === "desc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                          </span> */}
                        </span>
                      </th>
                      {userData?.role?.id === 1 && (
                        <>
                          <th
                            className="p-3 pr-9 relative text-left w-[200px]"
                            onClick={() => handleSorting("user.name")}
                            style={{ minWidth: "250px" }}
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
                          <th
                            className="w-36 p-3 pr-9 relative text-left"
                            /* onClick={() => handleSorting("readingTime")} */
                          >
                            <span className="grid w-24">
                              {t("readingTime")}
                              {/* <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                                <FaArrowUp
                                  className={
                                    sortColumn === "readingTime" &&
                                    sortDirection === "asc"
                                      ? "opacity-100"
                                      : " opacity-30"
                                  }
                                />
                                <FaArrowDown
                                  className={
                                    sortColumn === "readingTime" &&
                                    sortDirection === "desc"
                                      ? "opacity-100"
                                      : " opacity-30"
                                  }
                                />
                              </span> */}
                            </span>
                          </th>
                          <th
                            className="w-36 p-3 pr-9 relative text-left"
                            onClick={() =>
                              handleSorting("subscriptionRenewalDate")
                            }
                          >
                            <span className="block w-24">
                              {t("subscriptionStatus")}
                              <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                                <FaArrowUp
                                  className={
                                    sortColumn === "subscription" &&
                                    sortDirection === "asc"
                                      ? "opacity-100"
                                      : " opacity-30"
                                  }
                                />
                                <FaArrowDown
                                  className={
                                    sortColumn === "subscription" &&
                                    sortDirection === "desc"
                                      ? "opacity-100"
                                      : " opacity-30"
                                  }
                                />
                              </span>
                            </span>
                          </th>
                        </>
                      )}
                      <th className="w-36 p-3 pr-9 relative">{t("alerts")}</th>
                      {userData &&
                        userData?.role?.id !== 5 &&
                        userData?.role?.id !== 6 && (
                          <th className="p-3 relative w-44">{/* Options */}</th>
                        )}
                    </tr>
                  </thead>

                  {/* Table body */}
                  <tbody>
                    {binsData.map((bin) => (
                      <tr key={bin.id}>
                        <td>
                          <span className="grid w-10 place-items-center">
                            <input
                              type="checkbox"
                              className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
                              id={bin.id}
                              checked={selectedItems.includes(bin.id)}
                              onChange={() => handleSelectItem(bin.id)}
                            />
                          </span>
                        </td>
                        <td
                          className={` align-middle text-center text-white p-3 text-lg ${
                            bin.contentType.id === 1
                              ? "bg-themeColors-bgColor"
                              : bin.contentType.id === 2
                              ? "bg-themeColors-successColor"
                              : bin.contentType.id === 5
                              ? "bg-[#8423FF]"
                              : bin.contentType.id === 6
                              ? "bg-[#003CB1]"
                              : bin.contentType.id === 7
                              ? "bg-[#A67345]"
                              : bin.contentType.id === 8
                              ? "bg-[#3656FF]"
                              : bin.contentType.id === 9
                              ? "bg-[#FF238D]"
                              : bin.contentType.id === 10
                              ? "bg-[#D0D0D0]"
                              : bin.contentType.id === 11
                              ? "bg-[#FF7223]"
                              : bin.contentType.id === 12
                              ? "bg-[#82005E]"
                              : bin.contentType.id === 13
                              ? "bg-[#36B7FF]"
                              : bin.contentType.id === 14
                              ? "bg-[#FFDB1F]"
                              : bin.contentType.id === 15
                              ? "bg-[#3A2D2D]"
                              : bin.contentType.id === 16
                              ? "bg-[#00EEE0]"
                              : bin.contentType.id === 17
                              ? "bg-[#66EE00]"
                              : bin.contentType.id === 18
                              ? "bg-[#00857D]"
                              : bin.contentType.id === 19
                              ? "bg-[#2563EB]"
                              : ""
                          }`}
                        >
                          <span className=" w-full h-10 grid place-items-center mx-auto">
                            {bin.contentType.id === 1 ? (
                              <FaTrash size={30} />
                            ) : bin.contentType.id === 2 ? (
                              <FaRecycle size={30} />
                            ) : bin.contentType.id === 5 ? (
                              <DiscIcon width={40} />
                            ) : bin.contentType.id === 6 ? (
                              <BooksIcon width={40} />
                            ) : bin.contentType.id === 7 ? (
                              <FruitIcon width={40} />
                            ) : bin.contentType.id === 8 ? (
                              <LiquidIcon width={40} />
                            ) : bin.contentType.id === 9 ? (
                              <HouseholdIcon width={40} />
                            ) : bin.contentType.id === 10 ? (
                              <SprayIcon width={40} />
                            ) : bin.contentType.id === 11 ? (
                              <AppliancesIcon width={40} />
                            ) : bin.contentType.id === 12 ? (
                              <TextileIcon width={40} />
                            ) : bin.contentType.id === 13 ? (
                              <PlasticIcon width={20} />
                            ) : bin.contentType.id === 14 ? (
                              <GlassIcon width={20} />
                            ) : bin.contentType.id === 15 ? (
                              <PrinterIcon width={30} />
                            ) : bin.contentType.id === 16 ? (
                              <CrockeryIcon width={40} />
                            ) : bin.contentType.id === 17 ? (
                              <BatteryIcon width={40} />
                            ) : bin.contentType.id === 18 ? (
                              <CanIcon width={25} />
                            ) : bin.contentType.id === 19 ? (
                              <FaNewspaper size={30} />
                            ) : (
                              ""
                            )}
                          </span>
                        </td>
                        <td>
                          <Link
                            to={`/bins/${bin.id}`}
                            className=" text-themeColors-accentColor hover:text-themeColors-accentColorDark font-normal"
                          >
                            {bin.location}
                          </Link>
                        </td>
                        <td>{bin.serialNumber}</td>
                        <td
                          className={` align-middle text-center text-white p-3 text-lg ${
                            bin.fillLevelPercentage < bin.emptyLevel
                              ? "bg-themeColors-successColor"
                              : bin.fillLevelPercentage >= bin.emptyLevel &&
                                bin.fillLevelPercentage < bin.fullLevel
                              ? "bg-themeColors-warningColor"
                              : "bg-themeColors-dangerColor"
                          }`}
                        >
                          <span className=" w-full h-10 grid place-items-center mx-auto">
                            {bin.fillLevelPercentage} %
                          </span>
                        </td>
                        <td>
                          {bin.batteryVoltage
                            ? bin.batteryVoltage.toFixed(2) + " V"
                            : "-"}
                        </td>
                        <td>
                          {bin.temperature ? bin.temperature + " °C" : "-"}{" "}
                        </td>
                        {userData?.role?.id === 1 && (
                          <>
                            <td>{bin.customerName}</td>
                            <td>
                              {bin.lastUnitReadingCreateDate
                                ? formatDate(bin.lastUnitReadingCreateDate)
                                : "-"}
                            </td>
                            <td>
                              {bin.subscriptionRenewalDate
                                ? t("active")
                                : t("noSubscription")}
                            </td>
                          </>
                        )}
                        <td>
                          <div className="flex items-center justify-start gap-2">
                            {bin.alertTypes.length > 0 &&
                              bin.alertTypes.map((alert) => (
                                <label key={alert.id} title={alert.name}>
                                  {alert.id === 1 ? (
                                    <FaBatteryQuarter />
                                  ) : alert.id === 2 ? (
                                    <FaExclamationCircle />
                                  ) : alert.id === 3 ? (
                                    <FaBatteryQuarter className=" rotate-12" />
                                  ) : alert.id === 4 ? (
                                    <FaLock />
                                  ) : alert.id === 5 ? (
                                    <FaTrashAlt className="rotate-15" />
                                  ) : alert.id === 6 ? (
                                    <FaDoorOpen />
                                  ) : alert.id === 7 ? (
                                    <BiWindowOpen />
                                  ) : alert.id === 8 ? (
                                    <FaTrashRestoreAlt />
                                  ) : alert.id === 9 ? (
                                    <BinDamaged />
                                  ) : alert.id === 10 ? (
                                    <FaFileExport />
                                  ) : (
                                    <></>
                                  )}
                                </label>
                              ))}
                          </div>
                        </td>
                        {userData &&
                          userData?.role?.id !== 5 &&
                          userData?.role?.id !== 6 && (
                            <td>
                              <div className="flex gap-2">
                                {submittingStatus === bin.id ? (
                                  <LoaderSmall />
                                ) : (
                                  <label className="relative inline-flex items-center cursor-pointer">
                                    <input
                                      type="checkbox"
                                      className="sr-only peer"
                                      checked={bin.statusId !== 9}
                                      disabled={submittingStatus}
                                      onChange={() => handleBinStatus(bin.id)}
                                    />
                                    <div className="w-11 h-6 bg-white border border-themeColors-bgColor peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[11px] after:left-[2px] after:bg-themeColors-bgColor after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-themeColors-accentColor peer-checked:border-themeColors-accentColor peer-checked:after:bg-white"></div>
                                  </label>
                                )}

                                <Link
                                  to={`/bins/edit-bin/${bin.id}`}
                                  className="btn bg-themeColors-bgLightColor text-themeColors-textColorLight hover:bg-themeColors-accentColor hover:text-white aspect-square grid place-items-center"
                                >
                                  <FaEdit />
                                </Link>
                                <button
                                  onClick={() => handleDeleteModal(bin.id)}
                                  className="btn bg-themeColors-bgLightColor text-themeColors-textColorLight hover:bg-themeColors-accentColor hover:text-white aspect-square grid place-items-center"
                                >
                                  <FaTrashAlt />
                                </button>
                              </div>
                            </td>
                          )}
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
              {binsData && binsData.length > 0 && (
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

      {selectedItems.length > 0 && (
        <button
          className="flex items-center gap-3 text-white p-4 cursor-pointer bg-[#6295AC] rounded-sm fixed bottom-5 left-1/2 -translate-x-1/2 text-base font-medium"
          style={{ boxShadow: "0 3px 6px -1px rgba(0,0,0,.33)" }}
          onClick={handleExportModal}
        >
          <FaFileExport />
          {t("exportSelected")}
        </button>
      )}

      {exportModalActive && (
        <ExportModal
          modalType="bins-export"
          handleModal={handleExportModal}
          exportDataId={2}
          data={exportData}
          resetExportData={resetExportData}
        />
      )}

      {deleteModalActive && (
        <DeleteModal
          handleCancel={() => handleDeleteModal()}
          handleConfirm={handleDelete}
          submitting={deleteSubmitting}
        />
      )}
    </MainWrapper>
  );
};

export default BinsList;
