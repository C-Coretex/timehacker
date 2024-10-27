import { useEffect, useState } from 'react';
import axios from 'axios';
import useHandleError from './useHandleError';
// !!!TODO
// import authService from '../components/api-authorization/AuthorizeService'

const useQueryApiClient = ({
  request, 
  onSuccess, 
  onError, 
  onFinally, 
  enabled = true, 
  dontShowMessages = false 
}) => {
  const [receivedData, setReceivedData] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [isSuccess, setIsSuccess] = useState(false);
  const [isError, setIsError] = useState(false);

  const [handleError] = useHandleError();

  const enableOnMount = request?.enableOnMount; // For methods except 'GET'
  const disableOnMount = request?.disableOnMount; // For method 'GET'
  const method = request?.method || 'GET';

  useEffect(() => {
    if (!disableOnMount && (enableOnMount || method === 'GET')) {
      actualCall(request.url, request?.data, request?.method, request?.multipart);
    }
  }, [enabled, disableOnMount, enableOnMount]);

  const constructUrl = (urlParams = {}, query = {}) => {
    let url;
    
    url = request.url.replace(/\{([^}]+)\}/g, (match, key) => {
      return urlParams[key] !== undefined ? urlParams[key] : match;
    })

    const params = [];
    Object.keys(query).forEach(key => {
      if (query[key] !== undefined) {
        params.push(`${key}=${encodeURIComponent(query[key])}`);
      }
    })

    if (params.length) {
      const queryString = params.join('&');
      url += `${url.includes('?') ? '&' : '?'}${queryString}`;
    }

    return url;
  }

  const refetch = async (urlParams, query) => {
    const url = constructUrl(urlParams, query);

    try {
      const response = await actualCall(url, request?.data, request?.method, request?.multipart);
      return response;
    } catch (error) {
      return error;
    }
  };

  const appendData = async (data, urlParams, query) => {
    const url = constructUrl(urlParams, query);

    try {
      const response = await actualCall(url, data, request?.method, request?.multipart);
      return response;
    } catch (error) {
      return error;
    }
  };

  const actualCall = async (
    url,
    data = {},
    method = 'GET',
    multipart = false
  ) => {
    if (!enabled) {
      return;
    }

    setIsLoading(true);

    // const token = await authService.getAccessToken();
    const token = null;

    const requestConfig = {
      headers: !token ? {} : { 'Authorization': `Bearer ${token}` },
      url: url,
      method: method,
      ...(method === 'GET' ? { params: data } : { data: data }),
      responseType: multipart ? 'blob' : 'json'
    };

    try {
      const response = await axios.request(requestConfig);

      const responseContent = response.data;

      if (responseContent?.success === false) {
        throw responseContent;
      }

      setReceivedData(responseContent);
      setIsSuccess(true);
      onSuccess && onSuccess(responseContent); //Call onSuccess if set

      return responseContent;
    } catch (e) {
      const response = e.response;

      setIsError(true);

      const actualError = typeof response === 'object' && response.hasOwnProperty('data') ? response.data : e;

      onError && onError(actualError); //Call onError if set
      handleError(actualError, dontShowMessages); //hook for global handling of errors

      throw actualError;
    } finally {
      onFinally && onFinally(); //Call onFinally if set
      setIsLoading(false);
    }
  };

  return {
    data: receivedData,
    isLoading,
    isSuccess,
    isError,
    refetch,
    appendData
  }
}

export default useQueryApiClient;
