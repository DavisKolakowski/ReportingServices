import React, { createContext, useReducer, useContext, ReactNode } from 'react';
import { ReportingActionType } from '@/models/enums/reporting-action-type';
import { ReportModel, ReportParameterModel, Report } from '@/models';
import ReportingClient from '@/api';

interface ReportingState {
  report: ReportModel | null;
  reportDetails: Report | null;
  loading: boolean;
  error: string | null;
}

type Action =
  | { type: ReportingActionType.SetReport; payload: ReportModel }
  | { type: ReportingActionType.SetReportDetails; payload: Report }
  | { type: ReportingActionType.SetReportParameters; payload: ReportParameterModel[] }
  | { type: ReportingActionType.SetLoading; payload: boolean }
  | { type: ReportingActionType.SetError; payload: string | null };

const initialState: ReportingState = {
  report: null,
  reportDetails: null,
  loading: false,
  error: null,
};

const ReportingContext = createContext<{
  state: ReportingState;
  dispatch: React.Dispatch<Action>;
}>({
  state: initialState,
  dispatch: () => null,
});

const reportingReducer = (state: ReportingState, action: Action): ReportingState => {
  switch (action.type) {
    case ReportingActionType.SetReport:
      return { ...state, report: action.payload, loading: false };
    case ReportingActionType.SetReportDetails:
      return { ...state, reportDetails: action.payload, loading: false };
    case ReportingActionType.SetReportParameters:
      if (state.reportDetails) {
        return {
          ...state,
          reportDetails: {
            ...state.reportDetails,
            parameters: action.payload,
          },
          loading: false,
        };
      }
      return state;
    case ReportingActionType.SetLoading:
      return { ...state, loading: action.payload };
    case ReportingActionType.SetError:
      return { ...state, error: action.payload, loading: false };
    default:
      return state;
  }
};

export const ReportingProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  const [state, dispatch] = useReducer(reportingReducer, initialState);

  return (
    <ReportingContext.Provider value={{ state, dispatch }}>
      {children}
    </ReportingContext.Provider>
  );
};

export const useReportingStore = () => {
  const context = useContext(ReportingContext);
  if (!context) {
    throw new Error('useReportingStore must be used within a ReportingProvider');
  }
  const { state, dispatch } = context;

  const setReport = (report: ReportModel) => {
    dispatch({ type: ReportingActionType.SetReport, payload: report });
  };

  const getReportDetails = async (key: string) => {
    dispatch({ type: ReportingActionType.SetLoading, payload: true });
    try {
      const response = await ReportingClient.User.getReportDetails(key);
      dispatch({ type: ReportingActionType.SetReportDetails, payload: response.model });
    } catch (error) {
      dispatch({ type: ReportingActionType.SetError, payload: 'Failed to fetch report details' });
    }
  };

  const updateParameter = (param: { name: string; value: any }) => {
    if (state.reportDetails && state.reportDetails.parameters) {
      const updatedParameters = state.reportDetails.parameters.map((p) =>
        p.name === param.name ? { ...p, currentValue: param.value } : p
      );
      dispatch({ type: ReportingActionType.SetReportParameters, payload: updatedParameters });
    }
  };

  const executeReport = async () => {
    if (!state.reportDetails) return;
    dispatch({ type: ReportingActionType.SetLoading, payload: true });
    try {
      const response = await ReportingClient.User.executeReport({ model: state.reportDetails });
      dispatch({ type: ReportingActionType.SetReportDetails, payload: response.model });
    } catch (error) {
      dispatch({ type: ReportingActionType.SetError, payload: 'Failed to execute report' });
    }
  };

  const downloadReport = async () => {
    if (!state.reportDetails) return;
    dispatch({ type: ReportingActionType.SetLoading, payload: true });
    try {
      await ReportingClient.User.downloadReport({ model: state.reportDetails });
      dispatch({ type: ReportingActionType.SetLoading, payload: false });
    } catch (error) {
      dispatch({ type: ReportingActionType.SetError, payload: 'Failed to download report' });
    }
  };

  return {
    state,
    setReport,
    getReportDetails,
    updateParameter,
    executeReport,
    downloadReport,
  };
};
