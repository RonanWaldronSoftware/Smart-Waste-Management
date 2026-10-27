import React from "react";
import { logo } from "../../../assets";
import { Link } from "react-router-dom";
import { useTranslation } from "react-i18next";
import LoaderSmall from "../../../components/LoaderSmall";

const LoginForm = ({
  type,
  error,
  loading,
  formData,
  errorMessage,
  handleSubmit,
  handleChange,
  tokenError,
}) => {
  const { t } = useTranslation("translation");
  return (
    <form className="flex flex-col w-full my-auto" onSubmit={handleSubmit}>
      {/* Error message */}
      {error && (
        <p className="text-center pt-4 text-themeColors-dangerColor ">
          {errorMessage}
        </p>
      )}

      {/* Form content */}
      {tokenError ? (
        <div className="flex flex-col pt-10 gap-10">
          <Link
            to="/login"
            className="btn btn-new disabled:cursor-not-allowed disabled:opacity-70"
          >
            {t("login")}
          </Link>
        </div>
      ) : (
        <div className="flex flex-col pt-10 gap-6">
          {/* Inputs */}
          {(type === "login" || type === "forgot-password") && (
            <input
              type="text"
              name="email"
              id="email"
              placeholder={t("email")}
              className="login-input-new"
              value={formData.email}
              onChange={handleChange}
            />
          )}
          {(type === "login" || type === "reset-password") && (
            <input
              type="password"
              name="password"
              id="password"
              placeholder={t("password")}
              className="login-input-new"
              value={formData.password}
              onChange={handleChange}
            />
          )}
          {type === "reset-password" && (
            <input
              type="password"
              name="confirmPassword"
              id="confirmPassword"
              placeholder={t("confirmPassword")}
              className="login-input-new"
              value={formData.confirmPassword}
              onChange={handleChange}
            />
          )}

          <button
            type="submit"
            className="btn btn-new disabled:cursor-not-allowed disabled:opacity-70 min-h-[38px] mt-8"
            disabled={loading}
          >
            {loading ? (
              <LoaderSmall />
            ) : (
              <>
                {type === "login"
                  ? t("login")
                  : type === "reset-password"
                  ? t("confirmPassword")
                  : t("send")}
              </>
            )}
          </button>

          {type === "login" && (
            <Link
              to="/login/forgot-password"
              className=" text-white text-center text-sm font-medium"
            >
              {t("forgotPassword")}
            </Link>
          )}

          {type === "forgot-password" && (
            <Link to="/login" className="btn btn-secondary">
              {t("back")}
            </Link>
          )}
        </div>
      )}
    </form>
  );
};

export default LoginForm;
