import React, { useEffect, useState } from "react";
import { Loader, MainWrapper, NoResults, Pagination } from "../../components";
import { getUnitReadings } from "../../services/units";
import { Link, useNavigate, useParams } from "react-router-dom";
import { useSelector } from "react-redux";
import {
  FaArrowDown,
  FaArrowUp,
  FaSearch,
  FaFileExport,
  FaExclamationCircle,
  FaTrashAlt,
  FaDoorOpen,
  FaBatteryQuarter,
  FaLock,
  FaTrashRestoreAlt,
} from "react-icons/fa";
import { LiaAngleRightSolid } from "react-icons/lia";
import ExportModal from "../../components/ExportModal";
import { useTranslation } from "react-i18next";

import Calendar from "react-calendar";
import "react-calendar/dist/Calendar.css";
import { fetchWithRetry } from "../../utils/refreshAccessToken";
import { formatDate } from "../../utils/formatDate";
import LoaderSmall from "../../components/LoaderSmall";
import { BiWindowOpen } from "react-icons/bi";
import BinDamaged from "../../assets/icons/BinDamaged";

const BinHistoryReadings = () => {
  const { id } = useParams();
  const navigate = useNavigate();
  const params = useParams();
  const accessToken = useSelector((state) => state.user.accessToken);
  const { t } = useTranslation("translation");
  const [loading, setLoading] = useState(true);
  const [refetching, setRefetching] = useState(false);
  const [fetchError, setFetchError] = useState(false);
  const [binsData, setBinsData] = useState(null);
  const [binLocation, setBinLocation] = useState("");
  const [serialNo, setSerialNo] = useState(null);
  const [totalResults, setTotalResults] = useState(0);

  //pagination states
  const [itemsPerPage, setItemsPerPage] = useState(25);
  const [currentPage, setCurrentPage] = useState(1);
  const [startItem, setStartItem] = useState(null);
  const [endItem, setEndItem] = useState(null);

  //search state
  const [searchQuery, setSearchQuery] = useState("");

  //sorting states
  const [sortColumn, setSortColumn] = useState("ReadingDate");
  const [sortDirection, setSortDirection] = useState("desc");

  //checkbox selection
  const [selectedItems, setSelectedItems] = useState([]);

  const [datepickerActive, setDatepickerActive] = useState(false);
  const [datePickerValues, setDatePickerValues] = useState([
    new Date(),
    new Date(),
  ]);
  const [datePickerError, setDatePickerError] = useState(null);

  //export
  const [exportModalActive, setExportModalActive] = useState(false);
  const [exportData, setExportData] = useState([]);
  const [exportDates, setExportDates] = useState({
    from: null,
    to: null,
  });

  //data fetching
  useEffect(() => {
    setRefetching(true);
    fetchData();
  }, [itemsPerPage, currentPage, searchQuery, sortColumn, sortDirection]);

  //pagination update and filtering
  useEffect(() => {
    if (binsData && binsData.length > 0) {
      // Calculate the start and end indices of the items to be displayed on the current page
      const startIndex = (currentPage - 1) * itemsPerPage;
      const endIndex = startIndex + itemsPerPage;
      setStartItem((currentPage - 1) * itemsPerPage + 1);
      //calculate end item
      setEndItem(Math.min(currentPage * itemsPerPage, binsData.length));
    }
  }, [
    binsData,
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
        getUnitReadings,
        accessToken,
        navigate,
        t,
        params.id,
        "",
        searchQuery,
        currentPage,
        itemsPerPage,
        sortColumn,
        sortDirection
      );
      if (data.status === 200) {
        setBinsData(data.data.items);
        setBinLocation(data.data.location);
        setSerialNo(data.data.serialNumber);
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
  const handleDatepickerClose = () => {
    setDatepickerActive(false);
    setExportModalActive(true);
  };
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
    setDatePickerValues([]);
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
      <div className="flex flex-col w-full">
        {/* Header */}
        <div className="flex flex-wrap justify-between items-center bg-white py-3 px-6 mb-6">
          {/* Breadcrumbs */}
          <ul className="flex items-center gap-3 text-xs">
            <li>
              <Link
                to="/bins"
                className=" text-themeColors-accentColor hover:text-themeColors-accentColorDark"
              >
                {t("bins")}
              </Link>
            </li>
            <li>
              <LiaAngleRightSolid />
            </li>
            <li>
              {" "}
              <Link
                to={`/bins/${id}`}
                className=" text-themeColors-accentColor hover:text-themeColors-accentColorDark"
              >
                {binLocation}
              </Link>{" "}
            </li>
            <li>
              <LiaAngleRightSolid />
            </li>
            <li> {t("details")} </li>
          </ul>
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
                {/* date picker*/}
                <div className="relative">
                  <button
                    className="btn btn-primary"
                    onClick={() => setDatepickerActive((prev) => !prev)}
                  >
                    {t("export")}
                  </button>
                  {datepickerActive && (
                    <div
                      className="absolute top-full left-0 z-50 flex flex-col bg-white p-2 items-center"
                      style={{ boxShadow: "0 4px 16px rgba(0, 0, 0, 0.25)" }}
                    >
                      {datePickerError && <p>{datePickerError}</p>}
                      <Calendar
                        onChange={(values) => {
                          setExportDates({
                            from: values[0].toISOString(),
                            to: values[1].toISOString(),
                          });
                          setDatePickerValues(values);
                        }}
                        value={datePickerValues}
                        selectRange={true}
                      />
                      <div className="flex items-center gap-3 mt-2">
                        <button
                          className="btn btn-secondary"
                          onClick={() => {
                            setDatePickerValues([]);
                            setExportDates({
                              from: null,
                              to: null,
                            });
                          }}
                        >
                          {t("reset")}
                        </button>
                        <button
                          className="btn btn-primary"
                          onClick={handleDatepickerClose}
                        >
                          {t("confirm")}
                        </button>
                      </div>
                    </div>
                  )}

                  {datepickerActive && (
                    <div
                      onClick={() => setDatepickerActive(false)}
                      className="fixed top-0 left-0 w-screen h-screen"
                    />
                  )}
                </div>
                {refetching && <LoaderSmall />}
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
                <table className="table w-full min-w-[1300px]">
                  {/* Table header */}
                  <thead>
                    <tr>
                      <th className="p-3 w-16">
                        <span className="grid w-10 place-items-center">
                          <input
                            type="checkbox"
                            className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
                            checked={binsData.every((bin) =>
                              selectedItems.includes(bin.id)
                            )}
                            onChange={handleSelectAll}
                          />
                        </span>
                      </th>
                      <th
                        className=" p-3 pr-9 relative text-left"
                        onClick={() => handleSorting("binLevel")}
                      >
                        <span className="block">
                          {t("binLevel")}
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
                      {serialNo && !serialNo.startsWith("086") && (
                        <th
                          className="p-3 pr-9 relative text-left"
                          onClick={() => handleSorting("binLevelBC")}
                        >
                          {t("binLevelBC")}
                          <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                            <FaArrowUp
                              className={
                                sortColumn === "binLevelBC" &&
                                sortDirection === "asc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                            <FaArrowDown
                              className={
                                sortColumn === "binLevelBC" &&
                                sortDirection === "desc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                          </span>
                        </th>
                      )}
                      <th
                        className="p-3 pr-9 relative text-left"
                        onClick={() => handleSorting("NoFlapOpenings")}
                      >
                        <span className="text-left"> {t("flapOpenings")}</span>
                        <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                          <FaArrowUp
                            className={
                              sortColumn === "NoFlapOpenings" &&
                              sortDirection === "asc"
                                ? "opacity-100"
                                : " opacity-30"
                            }
                          />
                          <FaArrowDown
                            className={
                              sortColumn === "NoFlapOpenings" &&
                              sortDirection === "desc"
                                ? "opacity-100"
                                : " opacity-30"
                            }
                          />
                        </span>
                      </th>
                      <th
                        className=" p-3 pr-9 relative text-left"
                        onClick={() => handleSorting("batteryVoltage")}
                      >
                        <span className="grid w-[60px]">
                          {t("battery")}
                          <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                            <FaArrowUp
                              className={
                                sortColumn === "batteryVoltage" &&
                                sortDirection === "asc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                            <FaArrowDown
                              className={
                                sortColumn === "batteryVoltage" &&
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
                        onClick={() => handleSorting("temperature")}
                      >
                        <span className="block  whitespace-nowrap">
                          {t("temp")} °C
                          <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                            <FaArrowUp
                              className={
                                sortColumn === "temperature" &&
                                sortDirection === "asc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                            <FaArrowDown
                              className={
                                sortColumn === "temperature" &&
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
                        /* onClick={() => handleSorting("compactionNumber")} */
                      >
                        <span className="block  whitespace-nowrap">
                          {t("noCompactions")}
                          {/*  <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                            <FaArrowUp
                              className={
                                sortColumn === "compactionNumber" &&
                                sortDirection === "asc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                            <FaArrowDown
                              className={
                                sortColumn === "compactionNumber" &&
                                sortDirection === "desc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                          </span> */}
                        </span>
                      </th>
                      <th className="w-24 p-3 pr-9 relative">{t("alerts")}</th>
                      <th
                        className="p-3 pr-9 relative text-left"
                        onClick={() => handleSorting("ReadingDate")}
                      >
                        <span className="grid w-24">
                          {t("readingTime")}
                          <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                            <FaArrowUp
                              className={
                                sortColumn === "ReadingDate" &&
                                sortDirection === "asc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                            <FaArrowDown
                              className={
                                sortColumn === "ReadingDate" &&
                                sortDirection === "desc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                          </span>
                        </span>
                      </th>
                    </tr>
                  </thead>

                  {/* Table body */}
                  <tbody>
                    {binsData.map((reading) => (
                      <tr key={reading.id}>
                        <td>
                          <span className="grid w-10 place-items-center">
                            <input
                              type="checkbox"
                              checked={selectedItems.includes(reading.id)}
                              onChange={() => handleSelectItem(reading.id)}
                              className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
                            />
                          </span>
                        </td>
                        <td>{reading.binLevel}%</td>
                        {serialNo && !serialNo.startsWith("086") && (
                          <td>{reading.binLevelBc + "%"}</td>
                        )}
                        <td>{reading.noFlapOpenings}</td>
                        <td>
                          {reading.batteryVoltage
                            ? reading.batteryVoltage.toFixed(2) + " V"
                            : "-"}
                        </td>
                        <td>
                          {reading.temperature
                            ? reading.temperature + " °C"
                            : "-"}{" "}
                        </td>
                        <td>{reading.noCompactions}</td>
                        <td>
                          {reading?.alertTypes?.length > 0 &&
                            reading.alertTypes.map((alert) => (
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
                        </td>
                        <td>{formatDate(reading.readingDate)}</td>
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
          binId={params.id}
          handleModal={handleExportModal}
          modalType="history-export"
          exportDataId={3}
          data={exportData}
          resetExportData={resetExportData}
          exportDates={exportDates}
        />
      )}
    </MainWrapper>
  );
};

export default BinHistoryReadings;
