import { FocusSessionDto } from '../../../web-api-client';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import {
  prepareRecovery, resolveRecovery,
  type DistractionReportDto, type PrepareRecoveryRequest, type ResolveRecoveryRequest,
  completeFocusSession,
  getFocusSession,
  getFocusSessionHistory,
  recordFocusSessionReflection,
  reportDistraction,
  startFocusSession,
  type DistractionReason,
  type FocusSessionReflection,
} from './focusApi';

const focusSessionKeys = {
  history: ['focus-sessions', 'history'] as const,
  detail: (sessionId: number) => ['focus-sessions', sessionId] as const,
};

export function useFocusSessionHistory() {
  return useQuery({
    queryKey: focusSessionKeys.history,
    queryFn: getFocusSessionHistory,
  });
}

export function useFocusSession(sessionId: number) {
  return useQuery({
    queryKey: focusSessionKeys.detail(sessionId),
    queryFn: () => getFocusSession(sessionId),
    enabled: Number.isInteger(sessionId) && sessionId > 0,
  });
}

export function useStartFocusSession() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: startFocusSession,
    onSuccess: (session) => {
      if (session.id !== undefined) {
        queryClient.setQueryData<FocusSessionDto>(
          focusSessionKeys.detail(session.id),
          session,
        );
      }
      void queryClient.invalidateQueries({ queryKey: focusSessionKeys.history });
    },
  });
}

export function useCompleteFocusSession(sessionId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: () => completeFocusSession(sessionId),
    onSuccess: (session) => {
      queryClient.setQueryData<FocusSessionDto>(
        focusSessionKeys.detail(sessionId),
        session,
      );
      void queryClient.invalidateQueries({ queryKey: focusSessionKeys.history });
    },
  });
}

export function useReportDistraction(sessionId: number) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (input: { reason: DistractionReason; language: string }) =>
      reportDistraction(sessionId, input.reason, input.language),
    onSuccess: report => {
      queryClient.setQueryData<FocusSessionDto>(focusSessionKeys.detail(sessionId), session =>
        session ? new FocusSessionDto({ ...session, pendingRecovery: report }) : session);
    },
    onError: () => { void queryClient.invalidateQueries({ queryKey: focusSessionKeys.detail(sessionId) }); },
  });
}

export function usePrepareRecovery(sessionId: number) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (input: { id: number; request: PrepareRecoveryRequest }) =>
      prepareRecovery(sessionId, input.id, input.request),
    onSuccess: (report: DistractionReportDto) => {
      queryClient.setQueryData<FocusSessionDto>(focusSessionKeys.detail(sessionId), session =>
        session ? new FocusSessionDto({ ...session, pendingRecovery: report }) : session);
    },
    onError: () => { void queryClient.invalidateQueries({ queryKey: focusSessionKeys.detail(sessionId) }); },
  });
}

export function useResolveRecovery(sessionId: number) {
  const queryClient = useQueryClient();
  return useMutation({
    mutationFn: (input: { id: number; request: ResolveRecoveryRequest }) =>
      resolveRecovery(sessionId, input.id, input.request),
    onSuccess: session => {
      queryClient.setQueryData(focusSessionKeys.detail(sessionId), session);
      void queryClient.invalidateQueries({ queryKey: focusSessionKeys.history });
    },
    onError: () => { void queryClient.invalidateQueries({ queryKey: focusSessionKeys.detail(sessionId) }); },
  });
}

export function useRecordFocusSessionReflection(sessionId: number) {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: (reflection: FocusSessionReflection) =>
      recordFocusSessionReflection(sessionId, reflection),
    onSuccess: (session) => {
      queryClient.setQueryData<FocusSessionDto>(
        focusSessionKeys.detail(sessionId),
        session,
      );
      void queryClient.invalidateQueries({ queryKey: focusSessionKeys.history });
    },
  });
}
