import React, { useEffect, useState } from "react";
import { Loader, MainWrapper, NoResults, Pagination } from "../../components";
import { FaSearch, FaFileExport, FaTrashAlt } from "react-icons/fa";
import { useDispatch, useSelector } from "react-redux";
import { useNavigate } from "react-router-dom";
import {
  getAllUsers,
  changeUserPassword,
  deleteUser,
  changeUserStatus,
} from "../../services/users";
import { BiPlus, BiSolidLock } from "react-icons/bi";
import { FaArrowDown, FaArrowUp, FaEdit } from "react-icons/fa";
import UserModal from "./components/UserModal";
import { toast } from "react-toastify";
import PasswordModal from "./components/PasswordModal";

import ExportModal from "../../components/ExportModal";
import { useTranslation } from "react-i18next";
import { fetchWithRetry } from "../../utils/refreshAccessToken";
import DeleteModal from "../../components/DeleteModal";
import { logout } from "../../store/userSlice";
import LoaderSmall from "../../components/LoaderSmall";

const Users = () => {
  const accessToken = localStorage.getItem("smartWasteAccessToken");
  const currentUser = useSelector((state) => state.user.userData);
  const { t } = useTranslation("translation");
  const [loading, setLoading] = useState(true);
  const [refetching, setRefetching] = useState(false);
  const [fetchError, setFetchError] = useState(false);
  const [usersData, setUsersData] = useState(null);
  const [totalResults, setTotalResults] = useState(0);
  const [fetchAllUsers, setFetchAllUsers] = useState(false);
  const [statusSubmitting, setStatusSubmitting] = useState(null);

  //pagination states
  const [itemsPerPage, setItemsPerPage] = useState(25);
  const [currentPage, setCurrentPage] = useState(1);
  const [startItem, setStartItem] = useState(0);
  const [endItem, setEndItem] = useState(0);

  //search state
  const [searchQuery, setSearchQuery] = useState("");

  //sorting states
  const [sortColumn, setSortColumn] = useState("name");
  const [sortDirection, setSortDirection] = useState("asc");

  //modal states
  const [userModalActive, setUserModalActive] = useState(false);
  const [modalType, setModalType] = useState(null);
  const [modalFormData, setModalFormData] = useState({
    name: "",
    email: "",
    password: "",
    roleId: "",
    parentId: "",
    languageId: "",
    address1: "",
    address2: "",
    binLevelAlert: 0,
    city: "",
    countryId: "",
    phoneNumber: "",
  });

  //password modal
  const [passwordModalActive, setPasswordModalActive] = useState(false);
  const [passwordFormData, setPasswordFormData] = useState({
    email: "",
    newPassword: "",
    confirmPassword: "",
  });

  //delete modal
  const [deleteModalActive, setDeleteModalActive] = useState(false);
  const [deleteEmail, setDeleteEmail] = useState("");
  const [deleteSubmitting, setDeleteSubmitting] = useState(false);

  //checkbox selection
  const [selectedItems, setSelectedItems] = useState([]);

  //export
  const [exportModalActive, setExportModalActive] = useState(false);
  const [exportData, setExportData] = useState([]);

  const dispatch = useDispatch();
  const navigate = useNavigate();

  //data fetching
  useEffect(() => {
    setRefetching(true);
    fetchData();
  }, [
    fetchAllUsers,
    searchQuery,
    currentPage,
    itemsPerPage,
    sortColumn,
    sortDirection,
  ]);

  //pagination update and filtering
  useEffect(() => {
    if (usersData && usersData.length > 0) {
      //calculate start item
      setStartItem((currentPage - 1) * itemsPerPage + 1);
      //calculate end item
      setEndItem(Math.min(currentPage * itemsPerPage, totalResults));
    }
  }, [usersData]);

  //create user
  const fetchData = async () => {
    try {
      const data = await fetchWithRetry(
        getAllUsers,
        accessToken,
        navigate,
        t,
        searchQuery,
        currentPage,
        itemsPerPage,
        sortColumn,
        sortDirection,
        fetchAllUsers
      );
      if (data.status === 200) {
        setUsersData(data.data.items);
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

  //function for user modal toggling
  const handleUserModal = (type, userData) => {
    if (userModalActive) {
      setUserModalActive(false);
      setModalType(null);
      setModalFormData({});
    } else {
      setUserModalActive(true);
      setModalType(type);

      if (userData) {
        setModalFormData({
          id: userData.id,
          name: userData.name,
          email: userData.email,
          password: userData.password,
          roleId: userData.role.id,
          parentId: userData.parent?.id || "",
          languageId: userData.language.id,
          address1: userData.address1 || "",
          address2: userData.address2 || "",
          binLevelAlert: userData.binLevelAlert || 0,
          city: userData.city || "",
          countryId: userData.country.id,
          phoneNumber: userData.phoneNumber || "",
        });
      } else {
        setModalFormData({
          name: "",
          email: "",
          password: "",
          roleId: "",
          parentId: "",
          languageId: "",
          address1: "",
          address2: "",
          binLevelAlert: 0,
          city: "",
          countryId: "",
          phoneNumber: "",
        });
      }
    }
  };

  //change user status
  const handleUserStatus = async (userId) => {
    setStatusSubmitting(userId);
    try {
      const res = await fetchWithRetry(
        changeUserStatus,
        accessToken,
        navigate,
        t,
        userId
      );
      if (res.status === 200) {
        toast.success(t("usetStatusHasBeenSuccessfullyChanged"));
        fetchData();

        if (userId === currentUser.id) {
          dispatch(logout());
          navigate("/login");
        }
      }
    } catch (error) {
      toast.error(t("requestError"));
      console.error(error);
    } finally {
      setStatusSubmitting(null);
    }
  };

  //handle password modal
  const handlePasswordModal = (type, email) => {
    if (passwordModalActive) {
      setPasswordModalActive(false);
      setModalType(null);
      setPasswordFormData({ email: "", newPassword: "", confirmPassword: "" });
    } else {
      setPasswordModalActive(true);
      setModalType(type);
      setPasswordFormData((prev) => ({ ...prev, email: email }));
    }
  };
  const handlePasswordFormSubmit = async (values) => {
    try {
      const res = await fetchWithRetry(
        changeUserPassword,
        accessToken,
        navigate,
        t,
        passwordFormData.email,
        values.newPassword
      );
      if (res.status === 200) {
        toast.success(t("passwordSuccessfullyUpdated"));
        handlePasswordModal("");
        fetchData();

        if (passwordFormData.email === currentUser.email) {
          dispatch(logout());
          navigate("/login");
        }
      }
    } catch (error) {
      toast.error(t("requestError"));
      console.error(error);
    }
  };

  //handle delete modal
  const handleDeleteModal = (email) => {
    if (deleteModalActive) {
      setDeleteModalActive(false);
      setDeleteEmail("");
    } else {
      setDeleteModalActive(true);
      setDeleteEmail(email);
    }
  };
  const handleDeleteUser = async () => {
    setDeleteSubmitting(true);
    try {
      const res = await fetchWithRetry(
        deleteUser,
        accessToken,
        navigate,
        t,
        deleteEmail
      );
      if (res.status === 200) {
        toast.success(t("userDeletedSuccessfully"));
        handleDeleteModal();
        fetchData();

        if (deleteEmail === currentUser.email) {
          dispatch(logout());
          navigate("/login");
        }
      } else {
        handleDeleteModal();
      }
    } catch (error) {
      toast.error(t("requestError"));
      console.error(error);
    } finally {
      setDeleteSubmitting(false);
    }
  };

  //handling checkbox selection
  const handleSelectAll = () => {
    if (usersData.every((user) => selectedItems.includes(user.id))) {
      // If all items are already selected, unselect all.
      const filtered = selectedItems.filter(
        (item) => !usersData.some((user) => user.id === item)
      );
      setSelectedItems(filtered);
    } else {
      // Select all items on the current page.
      const newItems = usersData.map((item) => item.id);
      setSelectedItems((prev) => [...prev, ...newItems]);
    }
  };
  const handleSelectItem = (itemId) => {
    if (selectedItems.includes(itemId)) {
      // Item is already selected, so unselect it.
      const filtered = selectedItems.filter((id) => id !== itemId);
      setSelectedItems(filtered);
    } else {
      // Item is not selected, so select it.
      setSelectedItems((prev) => [...prev, itemId]);
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
            {t("users")}
            {refetching && <LoaderSmall />}
          </h1>

          {/* Additional filters */}
          <div className="flex items-center gap-2">
            <button
              className="btn btn-primary flex items-center"
              onClick={() => handleUserModal("create")}
            >
              <BiPlus />
              <span>{t("add")}</span>
            </button>
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
                {/* all users checkbox*/}
                <label className="relative inline-flex items-center cursor-pointer gap-2">
                  <input
                    type="checkbox"
                    className="sr-only peer"
                    checked={fetchAllUsers}
                    onChange={() => setFetchAllUsers((prev) => !prev)}
                  />
                  <div className="w-11 h-6 bg-white border border-themeColors-bgColor peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-themeColors-bgColor after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-themeColors-accentColor peer-checked:border-themeColors-accentColor peer-checked:after:bg-white"></div>
                  <span className=" whitespace-nowrap">{t("allUsers")}</span>
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
            <div className="table-wrapper overflow-auto w-full" id="binsTable">
              {usersData && usersData.length > 0 ? (
                <table className="table w-full" style={{ minWidth: "1350px" }}>
                  {/* Table header */}
                  <thead>
                    <tr>
                      <th className="p-3 w-16">
                        <span className="grid w-10 place-items-center">
                          <input
                            type="checkbox"
                            className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
                            checked={usersData.every((user) =>
                              selectedItems.includes(user.id)
                            )}
                            onChange={handleSelectAll}
                          />
                        </span>
                      </th>
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
                        onClick={() => handleSorting("roleId")}
                      >
                        {t("role")}
                        <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                          <FaArrowUp
                            className={
                              sortColumn === "roleId" && sortDirection === "asc"
                                ? "opacity-100"
                                : " opacity-30"
                            }
                          />
                          <FaArrowDown
                            className={
                              sortColumn === "roleId" &&
                              sortDirection === "desc"
                                ? "opacity-100"
                                : " opacity-30"
                            }
                          />
                        </span>
                      </th>
                      <th
                        className=" p-3 pr-9 relative text-left"
                        onClick={() => handleSorting("parentId")}
                      >
                        <span className="text-left"> {t("parent")}</span>
                        <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                          <FaArrowUp
                            className={
                              sortColumn === "parentId" &&
                              sortDirection === "asc"
                                ? "opacity-100"
                                : " opacity-30"
                            }
                          />
                          <FaArrowDown
                            className={
                              sortColumn === "parentId" &&
                              sortDirection === "desc"
                                ? "opacity-100"
                                : " opacity-30"
                            }
                          />
                        </span>
                      </th>
                      <th
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
                        className="w-28 p-3 pr-9 relative text-left"
                        onClick={() => handleSorting("statusId")}
                      >
                        <span className="block w-20">
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
                        className="w-28 p-3 pr-9 relative text-left"
                        onClick={() => handleSorting("countryId")}
                      >
                        <span className="block w-24">
                          {t("country")}
                          <span className="absolute bottom-1/2 translate-y-1/2 right-1 flex">
                            <FaArrowUp
                              className={
                                sortColumn === "countryId" &&
                                sortDirection === "asc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                            <FaArrowDown
                              className={
                                sortColumn === "countryId" &&
                                sortDirection === "desc"
                                  ? "opacity-100"
                                  : " opacity-30"
                              }
                            />
                          </span>
                        </span>
                      </th>

                      <th className="w-56 p-3 relative">{/* Options */}</th>
                    </tr>
                  </thead>

                  {/* Table body */}
                  <tbody>
                    {usersData.map((user) => (
                      <tr key={user.id}>
                        <td>
                          <span className="grid w-10 place-items-center">
                            <input
                              type="checkbox"
                              className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
                              checked={selectedItems.includes(user.id)}
                              onChange={() => handleSelectItem(user.id)}
                            />
                          </span>
                        </td>
                        <td className="align-middle p-3 overflow-hidden text-ellipsis ">
                          {user.name}
                        </td>
                        <td>{user.role.name}</td>
                        <td className=" overflow-hidden text-ellipsis">
                          {user.parent?.name}
                        </td>
                        <td className="align-middle p-3 overflow-hidden text-ellipsis">
                          {user.email}
                        </td>
                        <td>{user.status.name}</td>
                        <td>{user.country.name}</td>
                        <td>
                          <div className="flex gap-2">
                            {currentUser?.id !== user.id && (
                              <>
                                {statusSubmitting === user.id ? (
                                  <LoaderSmall />
                                ) : (
                                  <label className="relative inline-flex items-center cursor-pointer">
                                    <input
                                      type="checkbox"
                                      className="sr-only peer"
                                      checked={user.status.id !== 3}
                                      onChange={() => handleUserStatus(user.id)}
                                    />
                                    <div className="w-11 h-6 bg-white border border-themeColors-bgColor peer-focus:outline-none rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[11px] after:left-[2px] after:bg-themeColors-bgColor after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-themeColors-accentColor peer-checked:border-themeColors-accentColor peer-checked:after:bg-white"></div>
                                  </label>
                                )}
                              </>
                            )}

                            <button
                              onClick={() => handleUserModal("edit", user)}
                              className="btn ml-auto bg-themeColors-bgLightColor text-themeColors-textColorLight hover:bg-themeColors-accentColor hover:text-white aspect-square grid place-items-center"
                            >
                              <FaEdit />
                            </button>
                            <button
                              className="btn bg-themeColors-bgLightColor text-themeColors-textColorLight hover:bg-themeColors-accentColor hover:text-white aspect-square grid place-items-center"
                              onClick={() =>
                                currentUser?.id !== user.id
                                  ? handlePasswordModal(
                                      "adminReset",
                                      user.email
                                    )
                                  : handlePasswordModal("", user.email)
                              }
                            >
                              <BiSolidLock />
                            </button>
                            {currentUser?.id !== user.id && (
                              <button
                                onClick={() => handleDeleteModal(user.email)}
                                className="btn bg-themeColors-bgLightColor text-themeColors-textColorLight hover:bg-themeColors-accentColor hover:text-white aspect-square grid place-items-center"
                              >
                                <FaTrashAlt />
                              </button>
                            )}
                          </div>
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
              {usersData && usersData.length > 0 && (
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

      {userModalActive && (
        <UserModal
          modalType={modalType}
          handleModal={handleUserModal}
          modalFormData={modalFormData}
          fetchData={fetchData}
        />
      )}
      {passwordModalActive && (
        <PasswordModal
          modalType={modalType}
          handleModal={handlePasswordModal}
          modalFormData={passwordFormData}
          setModalFormData={setPasswordFormData}
          handleFormSubmit={handlePasswordFormSubmit}
        />
      )}
      {deleteModalActive && (
        <DeleteModal
          handleCancel={() => handleDeleteModal("")}
          handleConfirm={handleDeleteUser}
          submitting={deleteSubmitting}
        />
      )}
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
          modalType="users-export"
          handleModal={handleExportModal}
          exportDataId={1}
          data={exportData}
          resetExportData={resetExportData}
        />
      )}
    </MainWrapper>
  );
};

export default Users;
