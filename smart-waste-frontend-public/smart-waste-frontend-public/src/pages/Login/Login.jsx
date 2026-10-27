import React, { useEffect, useState } from "react";
import LoginLayout from "./components/LoginLayout";
import LoginForm from "./components/LoginForm";
import { useDispatch } from "react-redux";
import { useNavigate } from "react-router-dom";
import { setAccessToken, setIsLoggedIn } from "../../store/userSlice";
import { fetchLoginData } from "../../services/login";
import { useTranslation } from "react-i18next";

const Login = () => {
  const accessTokenExists = localStorage.getItem("smartWasteAccessToken");
  const dispatch = useDispatch();
  const navigate = useNavigate();
  //const userData = useSelector((state) => state.user.userData);
  //const isLoggedIn = useSelector((state) => state.user.isLoggedIn);
  const { t } = useTranslation("translation");

  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(false);
  const [errorMessage, setErrorMessage] = useState(null);
  const [formData, setFormData] = useState({
    email: "",
    password: "",
  });

  //if user is already logged in, redirect to dashboard
  useEffect(() => {
    if (accessTokenExists) {
      navigate("/");
    }
  }, []);

  const handleChange = (e) => {
    setError(false);
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    /* Validate input values */
    if (formData.email === "" || formData.password === "") {
      setError(true);
      setLoading(false);
      setErrorMessage(t("wrongEmailOrPass"));

      return;
    } else {
      try {
        const data = await fetchLoginData(formData);
        if (data.status === 200) {
          //setFetchedData(data);
          //if tehre is accessToken, redirect to dashboard
          if (data.data.jwtAuthResult && data.data.jwtAuthResult.accessToken) {
            dispatch(setAccessToken(data.data.jwtAuthResult.accessToken));
            localStorage.setItem(
              "smartWasteAccessToken",
              data.data.jwtAuthResult.accessToken
            );
            localStorage.setItem(
              "smartWasteRefreshToken",
              JSON.stringify(data.data.jwtAuthResult.refreshToken)
            );
            /* dispatch(setUserId(parsedUserData.id)); */

            setTimeout(() => {
              dispatch(setIsLoggedIn(true));
              navigate("/");
            }, 100);
          } else {
            //redirect user to setting new password
            localStorage.setItem("email", formData.email);
            localStorage.setItem("oneTimeToken", data.data.oneTimeToken);
            navigate("/login/reset-password");
          }
        } else {
          setError(true);
          setErrorMessage(t("wrongEmailOrPass"));
        }
      } catch (error) {
        //setLoginError(true);
        setLoading(false);
        setErrorMessage(t("requestError"));
        console.error(error);
      } finally {
        setLoading(false);
      }
    }
  };

  return (
    <LoginLayout>
      <LoginForm
        type="login"
        handleSubmit={handleSubmit}
        handleChange={handleChange}
        formData={formData}
        error={error}
        errorMessage={errorMessage}
        loading={loading}
      />
    </LoginLayout>
  );
};

export default Login;
