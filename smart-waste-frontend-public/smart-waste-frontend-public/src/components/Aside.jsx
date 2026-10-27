import React, { useEffect, useState } from "react";
import {
  BiCommentError,
  BiSolidCommentError,
  BiTachometer,
  BiTrash,
} from "react-icons/bi";
import { FaRegMap, FaRoute } from "react-icons/fa";
import { HiOutlineUsers } from "react-icons/hi";
import { useDispatch, useSelector } from "react-redux";
import { NavLink, useNavigate } from "react-router-dom";
import { useTranslation } from "react-i18next";
import { setIsExpanded } from "../store/navSlice";
import { getUser } from "../services/users";
import { fetchWithRetry } from "../utils/refreshAccessToken";
import { getNavCount } from "../services/lookup";

const Aside = () => {
  const navigate = useNavigate();
  const dispatch = useDispatch();
  const isExpanded = useSelector((state) => state.nav.isExpanded);
  const accessToken = localStorage.getItem("smartWasteAccessToken");
  const { t } = useTranslation("translation");

  const [userData, setUserData] = useState(null);
  const [binCount, setBinCount] = useState(null);
  const [damageCount, setDamageCount] = useState(null);
  /* 
  useEffect(() => {
    if (!userData) {
      logout();
      navigate("/login");
    }
  }, [userData]); */

  useEffect(() => {
    fetchUserData();
    fetchCount();
    const width = window.innerWidth;

    if (width > 1280) {
      dispatch(setIsExpanded(true));
    }
  }, []);

  const fetchUserData = async () => {
    try {
      const data = await fetchWithRetry(getUser, accessToken, navigate, t);

      if (data.status === 200) {
        setUserData(data.data);
      }
    } catch (error) {
      console.error(error);
    }
  };
  const fetchCount = async () => {
    try {
      const data = await fetchWithRetry(getNavCount, accessToken, navigate, t);
      if (data.status === 200) {
        setDamageCount(data.data.damage);
        setBinCount(data.data.unit);
      }
    } catch (error) {
      console.error(error);
    }
  };

  return (
    <div className={`left-sidebar  ${isExpanded ? "expanded" : ""}`}>
      <nav>
        <ul>
          <li>
            <NavLink
              to="/"
              className={({ isActive }) =>
                `sidebar-link ${isActive ? "active" : ""}`
              }
            >
              <BiTachometer size={18} />
              <span>{t("dashboard")} </span>
            </NavLink>
          </li>
          <li>
            <NavLink
              to="/map"
              className={({ isActive }) =>
                `sidebar-link ${isActive ? "active" : ""}`
              }
            >
              <FaRegMap size={18} />
              <span>{t("map")} </span>
            </NavLink>
          </li>
          <li>
            <NavLink
              to="/routes"
              className={({ isActive }) =>
                `sidebar-link ${isActive ? "active" : ""}`
              }
            >
              <FaRoute size={18} />
              <span>{t("routes")}</span>
            </NavLink>
          </li>
          <li className=" border-b border-solid border-themeColors-bgLightColor my-4"></li>
          <li>
            <NavLink
              to="/bins"
              className={({ isActive }) =>
                `sidebar-link relative ${isActive ? "active" : ""}`
              }
            >
              <BiTrash size={18} />
              <span>{t("bins")} </span>
              {binCount > 0 && (
                <span
                  className="absolute justify-center items-center text-white bg-themeColors-dangerColor rounded-full right-1 text-xs"
                  style={{ display: "flex", width: "32px", height: "32px" }}
                >
                  {binCount}
                </span>
              )}
            </NavLink>
          </li>
          <li>
            <NavLink
              to="/alerts"
              className={({ isActive }) =>
                `sidebar-link ${isActive ? "active" : ""}`
              }
            >
              <BiCommentError size={18} />
              <span>{t("alerts")} </span>
            </NavLink>
          </li>
          <li>
            <NavLink
              to="/reports"
              className={({ isActive }) =>
                `sidebar-link relative ${isActive ? "active" : ""}`
              }
            >
              <BiSolidCommentError size={18} />
              <span>{t("damage")} </span>
              {damageCount > 0 && (
                <span
                  className="absolute justify-center items-center text-white bg-themeColors-dangerColor rounded-full right-1 text-xs"
                  style={{ display: "flex", width: "32px", height: "32px" }}
                >
                  {damageCount}
                </span>
              )}
            </NavLink>
          </li>
          <li className=" border-b border-solid border-themeColors-bgLightColor my-4"></li>
          {/* {userData.role.name === "Admin" && (
              <li>
                <NavLink
                  to="/system"
                  className={({ isActive }) =>
                    `sidebar-link ${isActive ? "active" : ""}`
                  }
                >
                  <BiInfoCircle size={18} />
                  <span>{t("systemDetails")} </span>
                </NavLink>
              </li>
            )} */}
          {userData &&
            userData?.role?.id !== 5 && //don't show if its driver or technician
            userData?.role?.id !== 6 && (
              <li>
                <NavLink
                  to="/users"
                  className={({ isActive }) =>
                    `sidebar-link ${isActive ? "active" : ""}`
                  }
                >
                  <HiOutlineUsers size={18} />
                  <span>{t("users")} </span>
                </NavLink>
              </li>
            )}
          {/* {userData.role.name === "Corporate customer" && (
              <li>
                <NavLink
                  to="/users"
                  className={({ isActive }) =>
                    `sidebar-link ${isActive ? "active" : ""}`
                  }
                >
                  <HiOutlineUsers size={18} />
                  <span>{t("users")} </span>
                </NavLink>
              </li>
            )} */}
        </ul>
      </nav>
    </div>
  );
};

export default Aside;
