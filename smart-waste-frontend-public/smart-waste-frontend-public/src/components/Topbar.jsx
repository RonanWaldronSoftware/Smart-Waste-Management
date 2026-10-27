import React, { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { logoLight } from "../assets";
import { FaAngleDown, FaGlobeEurope, FaPowerOff, FaUser } from "react-icons/fa";
import { LiaBarsSolid } from "react-icons/lia";
import { useDispatch, useSelector } from "react-redux";
import { setIsExpanded } from "../store/navSlice";
import { logout, setUserData } from "../store/userSlice";
import { useTranslation } from "react-i18next";
import { langs } from "../i18n";
import { changeUserLanguage, getUser } from "../services/users";
import { fetchWithRetry } from "../utils/refreshAccessToken";
import { toast } from "react-toastify";

const Topbar = () => {
  const dispatch = useDispatch();
  const isExpanded = useSelector((state) => state.nav.isExpanded);
  const accessToken = localStorage.getItem("smartWasteAccessToken");
  const userData = useSelector((state) => state.user.userData);
  const configData = useSelector((state) => state.user.configData);
  const navigate = useNavigate();

  const [activeDropdown, setActiveDropdown] = useState(null);
  const { t, i18n } = useTranslation();

  useEffect(() => {
    if (userData?.language) {
      localStorage.setItem("i18nextLng", userData.language.code);
      i18n.changeLanguage(userData.language.code);
    } else {
      i18n.changeLanguage(localStorage.getItem("i18nextLng"));
    }
  }, [userData]);

  const handleMenuBtnClick = () => {
    dispatch(setIsExpanded(!isExpanded));
  };

  const handleDropdownBtnClick = (dropdown) => {
    if (!activeDropdown || activeDropdown !== dropdown) {
      setActiveDropdown(dropdown);
    } else {
      setActiveDropdown(null);
    }
  };
  const changeLanguage = (lng) => {
    i18n.changeLanguage(lng);
    updateUserLang(lng);
    setActiveDropdown(null); // Close the language dropdown after changing the language
  };

  const updateUserLang = async (lng) => {
    const chosenLng = configData.languages.find((lang) => lang.code === lng);
    const lngId = chosenLng?.id ? chosenLng?.id : 1;
    try {
      const res = await fetchWithRetry(
        changeUserLanguage,
        accessToken,
        navigate,
        t,
        lngId
      );
      if (res.status === 200) {
        localStorage.removeItem("smartWasteRefreshToken");
        localStorage.removeItem("smartWasteAccessToken");
        localStorage.setItem(
          "smartWasteRefreshToken",
          JSON.stringify(res.data.refreshToken)
        );
        localStorage.setItem("smartWasteAccessToken", res.data.accessToken);
        toast.success("Lang changed");
        fetchUserData();
      } else {
        toast.error("Lang not changed");
      }
    } catch (error) {
      console.error(error);
      toast.error("Lang not changed");
    }
  };
  const fetchUserData = async () => {
    try {
      const data = await fetchWithRetry(getUser, accessToken, navigate, t);
      if (data.status === 200) {
        dispatch(setUserData(data.data));
        setTimeout(() => {
          window.location.reload();
        }, 1000);
      }
    } catch (error) {
      console.error(error);
    }
  };

  const handleLogout = () => {
    dispatch(logout());
    navigate("/login");
  };

  return (
    <header className="fixed top-0 left-0 w-full bg-themeColors-bgColor flex items-center z-20">
      {/* Logo */}
      <div className="pl-3 min-w-[220px] w-[220px] min-h-[65px] flex items-center">
        <Link to="/" className="pl-4">
          <img src={logoLight} className="max-w-[110px]" />
        </Link>
      </div>
      <div className="w-full flex items-center justify-between">
        <button
          className="text-white opacity-50 text-lg px-4 h-full font-light hover:opacity-100"
          onClick={handleMenuBtnClick}
        >
          <LiaBarsSolid />
        </button>

        <ul className="flex">
          <li className="relative">
            <button
              className="topbar-btn"
              onClick={() => handleDropdownBtnClick("languages")}
            >
              <FaGlobeEurope />
              <FaAngleDown />
            </button>

            {activeDropdown === "languages" && (
              <ul className="dropdown-menu" style={{ minWidth: "auto" }}>
                {langs.map((lang) => (
                  <li
                    key={lang}
                    className="py-2 px-6 cursor-pointer text-themeColors-textColor font-normal text-base border-b border-solid border-themeColors-borderColor hover:bg-themeColors-bgLightColor uppercase"
                    onClick={() => changeLanguage(lang)}
                  >
                    {lang}
                  </li>
                ))}
              </ul>
            )}
          </li>
          <li className="relative">
            <button
              className="topbar-btn"
              onClick={() => handleDropdownBtnClick("user")}
            >
              <FaUser />
              <FaAngleDown />
            </button>

            {activeDropdown === "user" && (
              <div className="dropdown-menu py-2">
                <p className="py-2 px-6 font-normal text-base">
                  {userData?.email}
                </p>
                <span className="block my-2 border-b border-solid border-themeColors-borderColor" />
                <button
                  onClick={handleLogout}
                  className="py-2 px-6 cursor-pointer text-themeColors-textColor font-normal text-base  hover:bg-themeColors-bgLightColor w-full flex items-center gap-1"
                >
                  <FaPowerOff />
                  {t("logout")}
                </button>
              </div>
            )}
          </li>
        </ul>
      </div>

      {activeDropdown && (
        <div
          className="fixed w-screen h-screen top-0 left-0"
          onClick={() => setActiveDropdown(null)}
        ></div>
      )}
    </header>
  );
};

export default Topbar;
