import http from "../http-common";

const upload = (file: File, onUploadProgress: (progressEvent: any) => void): Promise<any> => {
  let formData = new FormData();

  formData.append("uploadedFile", file);

  return http.post("/api/stream", formData, {
    headers: {
      "Content-Type": "multipart/form-data",
      "Accept": "*/*",
      "User-Agent": "react-app",
      "X-SessionId": "111"
    },
    onUploadProgress,
  });
};

const getFiles = () : Promise<any> => {
  return http.get("/api/data/get");
};

const FileUploadService = {
  upload,
  getFiles,
};

export default FileUploadService;
