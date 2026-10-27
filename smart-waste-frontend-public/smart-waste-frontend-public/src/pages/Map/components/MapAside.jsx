import React from "react";
import { useTranslation } from "react-i18next";

const MapAside = ({
  filterMenuActive,
  checkboxFilters,
  handleCheckboxChange,
  unselectAllOwners,
  configData,
  owners,
}) => {
  const { t } = useTranslation("translation");
  return (
    <aside
      className={`absolute top-0 right-0 h-full bg-themeColors-bgLightColor transition-all py-4 border-l border-solid border-l-themeColors-borderColor w-[300px] ${
        filterMenuActive ? "translate-x-0" : "translate-x-[300px]"
      }`}
    >
      {/* Checkbox group */}
      <div
        className="flex flex-col p-6 pt-0 border-b border-solid"
        style={{ borderColor: "rgba(0,0,0,.1)" }}
      >
        <p className="mb-2">{t("wasteType")}</p>
        {configData?.contentTypes.map((item, i) => (
          <label
            key={item.id}
            htmlFor={item.id}
            className=" flex items-center gap-3"
          >
            <input
              type="checkbox"
              id={item.id}
              name="contentTypeIds"
              checked={checkboxFilters.contentTypeIds.includes(item.id)}
              onChange={() => handleCheckboxChange("contentTypeIds", item.id)}
              className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
            />
            <span>{item.name}</span>
          </label>
        ))}
        {/*  <label htmlFor="mixed" className=" flex items-center gap-3">
          <input
            type="checkbox"
            id="mixed"
            name="wasteType"
            checked={checkboxFilters.wasteType.includes("mixed")}
            onChange={() => handleCheckboxChange("wasteType", "mixed")}
            className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
          />
          <span>{t("mixWaste")}</span>
        </label>
        <label htmlFor="recycled" className=" flex items-center gap-3">
          <input
            type="checkbox"
            id="recycled"
            name="wasteType"
            checked={selectedCheckboxes.wasteType.includes("recyclable")}
            onChange={() => handleCheckboxChange("wasteType", "recyclable")}
            className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
          />
          <span>{t("recWaste")}</span>
        </label> */}
      </div>
      {/* Checkbox group */}
      <div
        className="flex flex-col p-6 border-b border-solid"
        style={{ borderColor: "rgba(0,0,0,.1)" }}
      >
        <p className="mb-2">{t("binCapacity")}</p>
        {configData?.status.map(
          (item, i) =>
            item.statusCategoryId === 4 && (
              <label
                key={item.id}
                htmlFor={item.id}
                className=" flex items-center gap-3"
              >
                <input
                  type="checkbox"
                  id={item.id}
                  name="fullnessStatusIds"
                  checked={checkboxFilters.fullnessStatusIds.includes(item.id)}
                  onChange={() =>
                    handleCheckboxChange("fullnessStatusIds", item.id)
                  }
                  className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
                />
                <span>{item.name}</span>
              </label>
            )
        )}
        {/* <label htmlFor="low" className=" flex items-center gap-3">
          <input
            type="checkbox"
            id="low"
            name="binCapacity"
            checked={selectedCheckboxes.binCapacity.includes("low")}
            onChange={() => handleCheckboxChange("binCapacity", "low")}
            className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
          />
          <span>{t("low")}</span>
        </label>
        <label htmlFor="medium" className=" flex items-center gap-3">
          <input
            type="checkbox"
            id="medium"
            name="binCapacity"
            checked={selectedCheckboxes.binCapacity.includes("medium")}
            onChange={() => handleCheckboxChange("binCapacity", "medium")}
            className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
          />
          <span>{t("medium")}</span>
        </label>
        <label htmlFor="high" className=" flex items-center gap-3">
          <input
            type="checkbox"
            id="high"
            name="binCapacity"
            checked={selectedCheckboxes.binCapacity.includes("high")}
            onChange={() => handleCheckboxChange("binCapacity", "high")}
            className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
          />
          <span>{t("high")}</span>
        </label> */}
      </div>
      {/* Checkbox group */}
      <div
        className="flex flex-col p-6 border-b border-solid"
        style={{ borderColor: "rgba(0,0,0,.1)" }}
      >
        <p className="mb-2">{t("deviceType")}</p>
        {configData?.unitTypes.map((item, i) => (
          <label
            key={item.id}
            htmlFor={item.id}
            className=" flex items-center gap-3"
          >
            <input
              type="checkbox"
              id={item.id}
              name="unitTypesIds"
              checked={checkboxFilters.unitTypesIds.includes(item.id)}
              onChange={() => handleCheckboxChange("unitTypesIds", item.id)}
              className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
            />
            <span>{item.name}</span>
          </label>
        ))}
        {/* <label htmlFor="nonCompacting" className=" flex items-center gap-3">
          <input
            type="checkbox"
            id="nonCompacting"
            name="deviceType"
            checked={selectedCheckboxes.deviceType.includes("Non-Compacting")}
            onChange={() =>
              handleCheckboxChange("deviceType", "Non-Compacting")
            }
            className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
          />
          <span>Non-Compacting</span>
        </label>
        <label htmlFor="compacting" className=" flex items-center gap-3">
          <input
            type="checkbox"
            id="compacting"
            name="deviceType"
            checked={selectedCheckboxes.deviceType.includes("Compacting")}
            onChange={() => handleCheckboxChange("deviceType", "Compacting")}
            className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
          />
          <span>Compacting</span>
        </label> */}
      </div>
      {/* Checkbox group - overflow */}
      <div
        className="flex flex-col p-6 border-b border-solid"
        style={{ borderColor: "rgba(0,0,0,.1)" }}
      >
        <p className="mb-2">{t("binModel")}</p>

        <div className="max-h-[110px] overflow-y-auto">
          {configData?.unitModelTypes.map((item, i) => (
            <label
              key={item.id}
              htmlFor={item.id}
              className=" flex items-center gap-3"
            >
              <input
                type="checkbox"
                id={item.id}
                name="unitModelTypeIds"
                checked={checkboxFilters.unitModelTypeIds.includes(item.id)}
                onChange={() =>
                  handleCheckboxChange("unitModelTypeIds", item.id)
                }
                className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
              />
              <span>{item.name}</span>
            </label>
          ))}

          {/* {checkboxFilters.binModel.map((model) => (
            <label
              key={model}
              htmlFor={model}
              className=" flex items-center gap-3"
            >
              <input
                type="checkbox"
                id={model}
                name="model"
                checked={selectedCheckboxes.binModel.includes(model)}
                onChange={() => handleCheckboxChange("binModel", model)}
                className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
              />
              <span>{model}</span>
            </label>
          ))} */}
        </div>
      </div>
      {/* Checkbox group - overflow */}
      <div
        className="flex flex-col p-6 border-b border-solid"
        style={{ borderColor: "rgba(0,0,0,.1)" }}
      >
        <div className="flex items-center justify-between">
          <p className="mb-2">{t("binOwners")}</p>

          <button onClick={unselectAllOwners}>{t("unselectAll")}</button>
        </div>

        <div className="max-h-[110px] overflow-y-auto">
          {owners.map((item, i) => (
            <label
              key={item.id}
              htmlFor={item.id}
              className=" flex items-center gap-3"
            >
              <input
                type="checkbox"
                id={item.id}
                name="userIds"
                checked={checkboxFilters.userIds.includes(item.id)}
                onChange={() => handleCheckboxChange("userIds", item.id)}
                className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
              />
              <span>{item.name}</span>
            </label>
          ))}
          {/* {checkboxFilters.binOwner.map((owner) => (
            <label
              key={owner}
              htmlFor={owner}
              className=" flex items-center gap-3"
            >
              <input
                type="checkbox"
                id={owner}
                name="owner"
                checked={selectedCheckboxes.binOwner.includes(owner)}
                onChange={() => handleCheckboxChange("binOwner", owner)}
                className="w-4 h-4 text-blue-600 bg-white border-black rounded accent-themeColors-accentColor"
              />
              <span>{owner}</span>
            </label>
          ))} */}
        </div>
      </div>
    </aside>
  );
};

export default MapAside;
