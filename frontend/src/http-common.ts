import axios from "axios";

export default axios.create({
  baseURL: "https://localhost:44395",
  headers: {
    "Content-type": "application/json",
  },
});
